using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SystemTestService.AppHitsService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using SystemTestService.OpsConsoleService;
using TR.AppServer.Common.Interfaces;
using TR.AppServer.Logging;
using Wcf.Routing;
using System.Reflection;
using System.Runtime.Caching;

namespace SystemTestService
{
    public static class StringExt
    {
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        public static string TrimToEnd(this string value, string trimStr)
        {
            var index = value.IndexOf(trimStr);
            return index > 0 ? value.Substring(0, index) : value;
        }
    }

    public class UpdateOpsConsoleDb: IDisposable
    {
        private static readonly ILogger ALogger = Logger.Default;

        private static ManualResetEvent _mre;
        private static ManualResetEvent _mre2;
        private static readonly int _checkFileTimeoutMs = 5000;
        private static readonly int _flushqueueLimit = 10000;

        private static Metadata _metadata;
        private bool _disposed;

        private static ConcurrentFlushQueue<LogStatItem> _flushQueue;
        private static readonly Regex _guidValidator =
            new Regex(@"\A[a-fA-F0-9]{8}(?:-[a-fA-F0-9]{4}){3}-[a-fA-F0-9]{12}\b");
        private const string zeroGuid = "00000000-0000-0000-0000-000000000000";
        private const string test127MSPRETXIII          = "127MSPRETXIII";
        private const string test127MSPRETXIII_Modified = "127MS1";
        private const string test127MSPRETXIV           = "127MSPRETXIV";
        private const string test127MSPRETXIV_Modified  = "127MS2";
        private const string test127MSSBXIII            = "127MSSBXIII";
        private const string test127MSSBXIII_Modified   = "127MS3";
        private const string test127MSPINSXIV           = "127MSPINSXIV";
        private const string test127MSPINSXIV_Modified  = "127MS4";

        public const string ProductName = "est";
        private const string TimestampFormat = "yyyy-MM-ddTHH:mm:ss.000Z";
        private static readonly string[] Ordinals = { "ms", "", "k", "m", "g", "t" };

        private static MemoryCache Cache = MemoryCache.Default;
        private const string MetaDataCacheKey = "OpsConsole-MetaData-{0}";
        private const int MetaDataCacheTimeoutSecs = 60 * 60;
        private volatile static int threadCount = 0;
        private static Object threadCountLock = new Object();

        static UpdateOpsConsoleDb()
        {
            int flushIntervalSeconds = 10;
#if DEBUG
            flushIntervalSeconds = 5;
#endif
            _mre2 = new ManualResetEvent(false);
            _flushQueue = new ConcurrentFlushQueue<LogStatItem>(_flushqueueLimit, TimeSpan.FromSeconds(flushIntervalSeconds));
            _flushQueue.RegisterFlushHandler(FlushQueueToOpsConsole);

            _mre = new ManualResetEvent(false);
        }

        public async static void SendDataToOpsConsole(List<LogStatItem> testResults)
        {
            _metadata = GetMetadata();

            if (_metadata == null)
            {
                ALogger.LogInfo("SendDataToOpsConsole: There is no metadata to validate the data before sending them to OpsConsoleService.");
                return;
            }

            while (threadCount > Utility.ConfigUtil.ThreadLimit || !_mre.WaitOne(_checkFileTimeoutMs))
            {
                if (threadCount >= Utility.ConfigUtil.ThreadLimit)
                {
                    ALogger.LogTrace("SendDataToOpsConsole: threadCount limit reach {0}", threadCount);
                    continue;
                }
                break;
            }

            ChangeThreadCount(1);
            await Task.Run(() => CreateBatch(testResults));
        }

        private static void ChangeThreadCount(int i)
        {
            lock (threadCountLock)
            {
                threadCount += i;
                ALogger.LogInfo("ChangeThreadCount: Thread Count is changed {0} : {1}",i,threadCount);
            }
        }

        private static string ShortenTest127LongName(string testName)
        {
            if (testName.Length <= 10)
            {
                return testName;
            }

            switch (testName)
            {
                case test127MSPRETXIII:
                    return test127MSPRETXIII_Modified;
                case test127MSPRETXIV:
                    return test127MSPRETXIV_Modified;
                case test127MSSBXIII:
                    return test127MSSBXIII_Modified;
                case test127MSPINSXIV:
                    return test127MSPINSXIV_Modified;
            }

            ALogger.LogWarn("ShortenTest127LongName: Long name cannot be modified: {0}", testName);
            return testName.Truncate(10);
        }

        public static List<LogStatItem> ConvertToLogStat(IReadOnlyCollection<TestResult> records)
        {
            if (records == null)
            {
                return null;
            }

            var uUid = "";
            var logStatList = new List<LogStatItem>();

            foreach (var item in records)
            {
                try
                {
                    var testCaseFailIdList = new List<string>();
                    var testCaseAlertIdList = new List<string>();
                    var testCaseWarnIdList = new List<string>();
                    var testCaseFailIds = new StringBuilder();
                    var testCaseAlertIds = new StringBuilder();
                    var testCaseWarnIds = new StringBuilder();
                    var total = 0;
                    var pass = 0;
                    var info = 0;
                    var warning = 0;
                    var alert = 0;
                    var fail = 0;

                    var tmpLogStat = new LogStatItem
                    {
                        timeStamp = Convert.ToString(item.ServerDbsDateTime),
                        uuid = item.UUID,
                        machineGuid = GuidValidator(item.MachineID),
                        installGuid = zeroGuid,
                        product = ProductName
                    };

                    if (IsEWM(item))
                    {
                        ALogger.LogInfo("ConvertToLogStat: EWM Mapping {0}: {1}-{2}-{3}", item.UUID, item.ProdName, item.ProdVers, item.TestCases.Count);
                        DataMapper.MapStatCodeEWM(item);
                        logStatList.Add(CreateLogStatInstance("isewm", "1", "", tmpLogStat));
                    }
                    else
                    {
                        logStatList.Add(CreateLogStatInstance("isewm", "0", "", tmpLogStat));
                    }

                    foreach (var testCase in item.TestCases)
                    {
                        total++;
                        var name = testCase.ID.Replace("#test.", "");
                        #region process validation
                        switch (testCase.Valid)
                        {
                            case "pass": pass++; break;
                            case "fail": fail++; 
                                if (testCaseFailIds.Length > 0 && testCaseFailIds[testCaseFailIds.Length - 1] != ',')
                                    testCaseFailIds.Append(",");
                                if (name == "24")
                                {
                                    name = "CMPNAME";
                                }
                                testCaseFailIds.Append(name);
                                if (testCaseFailIds.Length >= 30 || (total == item.TestCases.Count))
                                {
                                    testCaseFailIdList.Add(testCaseFailIds.ToString());
                                    testCaseFailIds.Clear();
                                    testCaseFailIds.Append(",");
                                }
                                break;
                            case "alert": alert++;
                                if (testCaseAlertIds.Length > 0 && testCaseAlertIds[testCaseAlertIds.Length - 1] != ',')
                                    testCaseAlertIds.Append(",");
                                if (name == "24")
                                {
                                    name = "CMPNAME";
                                }
                                testCaseAlertIds.Append(name);
                                if (testCaseAlertIds.Length >= 30 || (total == item.TestCases.Count))
                                {
                                    testCaseAlertIdList.Add(testCaseAlertIds.ToString());
                                    testCaseAlertIds.Clear();
                                    testCaseAlertIds.Append(",");
                                }
                                break;
                            case "warning": warning++;
                                if (testCaseWarnIds.Length > 0 && testCaseWarnIds[testCaseWarnIds.Length - 1] != ',')
                                    testCaseWarnIds.Append(",");
                                if (name == "24")
                                {
                                    name = "CMPNAME";
                                }
                                testCaseWarnIds.Append(name);
                                if (testCaseWarnIds.Length >= 30 || (total == item.TestCases.Count))
                                {
                                    testCaseWarnIdList.Add(testCaseWarnIds.ToString());
                                    testCaseWarnIds.Clear();
                                    testCaseWarnIds.Append(",");
                                }
                                break;
                            case "info": info++; break;
                        }
                        #endregion process validation

                        #region process test case
                        if (name == "24")
                        {
                            name = "CMPNAME";
                        }
                        var value = testCase.Value;
                        var desc = testCase.Description;
                                                    
                        if (name == "127" && !string.IsNullOrEmpty(value))
                        {
                            const string pattern = @"([\D^.,]*)([\d|.]*),";
                            var rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                            var matches = rgx.Matches(value);

                            logStatList.AddRange(from Match match in matches
                                                    where match.Success
                                                    let testName = match.Groups[1].ToString()
                                                    let testValue = match.Groups[2].ToString()
                                                    select CreateLogStatInstance(ShortenTest127LongName(name + testName), testValue, desc, tmpLogStat) into logStat
                                                    where logStat != null
                                                    select logStat);
                        }
                        else if (name == "142")
                        {
                            const string pattern = @"\{(Primary Screen=(\w+),\{x=(-*\d+),y=(-*\d+),width=(-*\d+),height=(-*\d+)\})\}+";
                            var rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                            var matches = rgx.Matches(value);

                            var i = 0;
                            logStatList.AddRange(from Match match in matches
                                                    where match.Success
                                                    let newName = String.Format("{0}.{1}", name, ++i)
                                                    let newValue = String.Format("{0}{1},{2},{3},{4}", match.Groups[2].Value.Equals("True", StringComparison.CurrentCultureIgnoreCase) ? "Primary " : "",
                                                    match.Groups[3].Value, match.Groups[4].Value, match.Groups[5].Value, match.Groups[6].Value)
                                                    select CreateLogStatInstance(newName, newValue, desc, tmpLogStat) into logStat
                                                    where logStat != null
                                                    select logStat);
                        }
                        else
                        {
                            if (name == "48") // TODO: consider to apply this logic if "@" can be removed from all test case
                            {
                                desc = desc.TrimToEnd("@");
                            }

                            value = Regex.Replace(value, @"\s+", " ");
                            desc = Regex.Replace(desc, @"\s+", " ");
                            desc = desc.Trim();
                            var logStat = CreateLogStatInstance(name.Truncate(10), value, desc, tmpLogStat);
                            if (logStat != null)
                                logStatList.Add(logStat);
                        }
                        #endregion
                    }
                    #region add test cases
                    // Fail
                    if (testCaseFailIds.Length > 1)
                    {
                        testCaseFailIdList.Add(testCaseFailIds.ToString());
                    }
                    if (testCaseAlertIds.Length > 1)
                    {
                        testCaseAlertIdList.Add(testCaseAlertIds.ToString());
                    }
                    if (testCaseWarnIds.Length > 1)
                    {
                        testCaseWarnIdList.Add(testCaseWarnIds.ToString());
                    }

                    var other = total - pass - info - warning - alert - fail;
                    if(item.RunningMode!=null) logStatList.Add(CreateLogStatInstance("runmode", item.RunningMode, "", tmpLogStat));
                    logStatList.Add(CreateLogStatInstance("total", total.ToString(), "", tmpLogStat));
                    logStatList.Add(CreateLogStatInstance("pass", pass.ToString(), "", tmpLogStat));
                    logStatList.Add(CreateLogStatInstance("info", info.ToString(), "", tmpLogStat));
                    AddValidationStat(testCaseFailIdList, ref logStatList, tmpLogStat, "fail", fail);
                    AddValidationStat(testCaseAlertIdList, ref logStatList, tmpLogStat, "alert", alert);
                    AddValidationStat(testCaseWarnIdList, ref logStatList, tmpLogStat, "warn", warning);
                    logStatList.Add(CreateLogStatInstance("other", other.ToString(), "", tmpLogStat));

                    if (item.ServerCompName != null) logStatList.Add(CreateLogStatInstance("servname", item.ServerCompName, "", tmpLogStat));
                    if (item.ServerDatacenter != null) logStatList.Add(CreateLogStatInstance("envname", item.ServerDatacenter, "", tmpLogStat));
                    logStatList.Add(CreateLogStatInstance("oldid", item.TestID.ToString(), "", tmpLogStat));
                    #endregion add test cases
                }
                catch (Exception ex)
                {
                    ALogger.LogError("ConvertToLogStat: error processing item user {0}: {1}", uUid, ex.Message);
                }
            }

            ALogger.LogInfo("ConvertToLogStat: Done convert stat {0}", logStatList.Count);
            return logStatList;

        }

        public static void AddValidationStat(List<string> validationIDList, ref List<LogStatItem> logStatList, LogStatItem tmpLogStat, string valid, int total)
        {            
            var i = 0;
            if (validationIDList.Count == 0 || total == 0)
            {
                logStatList.Add(CreateLogStatInstance(valid, "0", "", tmpLogStat));
            }
            else
            {
                foreach (var varidateIds in validationIDList)
                {
                    if (i == 0)
                    {
                        if (i == validationIDList.Count - 1)
                            logStatList.Add(CreateLogStatInstance(valid, total + "[" + varidateIds + "]", "", tmpLogStat));
                        else
                            logStatList.Add(CreateLogStatInstance(valid, total + "[" + varidateIds, "", tmpLogStat));
                    }
                    else
                    {
                        if (i == validationIDList.Count - 1)
                            logStatList.Add(CreateLogStatInstance(valid + "." + i, varidateIds + "]", "", tmpLogStat));
                        else
                            logStatList.Add(CreateLogStatInstance(valid + "." + i, varidateIds, "", tmpLogStat));
                    }
                    i++;
                }
            }
        }

        private static Metadata GetMetadata()
        {
            try
            {
                ALogger.LogInfo("GetMetadata: getting metadata");

                var metadataObj = Cache.Get(MetaDataCacheKey) as Metadata;

                if(metadataObj != null && metadataObj.enumdict != null && metadataObj.enumdict.Count < 1)
                {
                    ALogger.LogInfo("GetMetadata: get from cache");
                    return metadataObj;
                }

                using (var client = new OpsConsoleServiceClient(RouterBindings.Local, RouterAddresses.Local.RequestReply))
                {
                    var request = new GetMetadataRequest()
                    {
                        product = "est"
                    };
                    var metadataJson = client.GetMetadata(request);

                    metadataObj =  JsonConvert.DeserializeObject<Metadata>(metadataJson);

                    if (metadataObj.enumdict != null && metadataObj.enumdict.Count < 1)
                    {
                        metadataObj = GetLocalMetadata();
                    }

                    Cache.Set(MetaDataCacheKey, metadataObj, DateTimeOffset.UtcNow.AddSeconds(MetaDataCacheTimeoutSecs));
                    return metadataObj;
                }
            }
            catch (Exception ex)
            {
                ALogger.LogError("GetMetadata: Error getting/parsing metadata: {0}", ex.Message);
                return GetLocalMetadata();
            }
        }

        private static Metadata GetLocalMetadata()
        {
            try
            {
                using (var sStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SystemTestService.Schema.json"))
                {
                    using (var reader = new StreamReader(sStream))
                    {
                        sStream.Seek(0, SeekOrigin.Begin);
                        var metadataJson = reader.ReadToEnd();
                        return JsonConvert.DeserializeObject<Metadata>(metadataJson);
                    }
                }
            }
            catch (Exception ex2)
            {
                ALogger.LogError("GetMetadata: cannot get metadata from resource file, {1}", ex2.Message);
                return null;
            }
        }


        private static LogStatItem CreateLogStatInstance(string name, string value, string description, LogStatItem tmp)
        {
            var testCase = new LogStatItem {
                statName = name,
                statVal = value,
                uuid = tmp.uuid,
                timeStamp = tmp.timeStamp,
                machineGuid = tmp.machineGuid,
                product = tmp.product,
                installGuid = tmp.installGuid
            };
            if (_metadata == null)
            {
                _metadata = GetMetadata();
            }

            var index = _metadata.testschema.FindIndex(s => s.name == name);
            if (index == -1)
            {
                ALogger.LogWarn("Cannot find metadata for [{0}]", name);
                return null;
            }
            var schema = _metadata.testschema[index];
            if (!schema.enabled)
            {
                return null;
            }

            if (schema.usedesc || string.IsNullOrWhiteSpace(value))
            {
                if (!string.IsNullOrWhiteSpace(description) && description != "-")
                {
                    testCase.statVal = description;
                }
            }

            if (schema.type == "boolean")
            {
                testCase.statVal = ParseBoolean(testCase.statVal);
                return testCase;
            }

            if (!string.IsNullOrEmpty(schema.unit))
            {
                testCase.statVal = ParseUnit(testCase.statVal, schema.unit);
                return testCase;
            }

            if (testCase.statVal.Length <= 40) return testCase;

            var mapCode = _metadata.enumdict.Find(s => s.Test.Contains(name));
            if (mapCode != null)
            {
                foreach (var map in mapCode.Map)
                {
                    testCase.statVal = ReplaceCaseInsensitive(testCase.statVal, map.Value, "#" + map.ID, map.Display == null);
                    if (testCase.statVal.Length <= 40) return testCase;
                }
            }

            if (testCase.statVal.Length <= 40) return testCase;
            //suppress warning message for some stats such as userID, email.
            var longStatIgnoreList = new List<string>() {"61","118"};
            if (!longStatIgnoreList.Contains(testCase.statName))
            {
                ALogger.LogWarn("[{0}] value length is over 40 characters, [{1}]", testCase.statName, testCase.statVal);
            }            
            testCase.statVal = testCase.statVal.Substring(0, 37) + "...";

            return testCase;
        }

        private static string ReplaceCaseInsensitive(string input, string search, string replacement, bool isEscape = true)
        {
            string result = Regex.Replace(
                input,
                isEscape ? Regex.Escape(search) : search,
                replacement.Replace("$", "$$"),
                RegexOptions.IgnoreCase
            );
            return result;
        }

        private static string ParseUnit(string value, string schemaUnit)
        {
            if (value.Equals("N/A", StringComparison.InvariantCultureIgnoreCase))
            {
                return "0";
            }
            const string pattern = @"([\d|.]+)[\s-]*([\w^d]*)\W*";
            var lowerSchemaUnit = schemaUnit.ToLower();
            try
            {
                var rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                var matches = rgx.Matches(value.ToLower());

                if (matches[0].Success)
                {
                    var testValue = matches[0].Groups[1].ToString();
                    var testUnit = matches[0].Groups[2].ToString();
                    var testUnitPrefix = GetUnitPrefix(testUnit);
                    var schemaUnitPrefix = GetUnitPrefix(lowerSchemaUnit);

                    if (testUnit.Equals(lowerSchemaUnit))
                        return testValue;
                    float floatValue;
                    if (!float.TryParse(testValue, out floatValue))
                        return "0";

                    var valueInSmallestUnit = GetValueInSmallestUnit(floatValue, testUnitPrefix);
                    return ConvertUnit(valueInSmallestUnit, schemaUnitPrefix);
                }
            }
            catch (Exception ex)
            {
                ALogger.LogWarn("ParseUnit: [{0}!{1}], {2}", value, schemaUnit, ex.Message);
            }
            return "0";
        }

        private static string GetUnitPrefix(string unit)
        {
            if (string.IsNullOrWhiteSpace(unit))
                return "";
            if (unit.StartsWith("ms"))
                return "ms";
            var prefix = unit.Substring(0, 1);

            return Ordinals.Any(prefix.Equals) ? prefix : "";
        }

        private static long GetValueInSmallestUnit(float floatValueInGivenUnit, string givenUnit)
        {

            var index = Array.FindIndex(Ordinals, s => s.Equals(givenUnit));
            var newVal = floatValueInGivenUnit;

            while (index-- > 1)
            {
                newVal *= 1024;
            }

            return (long)newVal;
        }

        private static string ConvertUnit(long valueInSmallestUnit, string expectedUnit)
        {
            var index = Array.FindIndex(Ordinals, s => s.Equals(expectedUnit));

            var rate = (decimal)valueInSmallestUnit;

            var ordinal = 0;
            while (++ordinal < index)
            {
                rate /= 1024;
            }
            var newVal = Math.Round(rate, 2, MidpointRounding.AwayFromZero);
            return newVal.ToString();
        }

        private static string ParseBoolean(string value)
        {
            const string code0 = "code 0"; // except value that we consider as true (1)
            var valueInLower = value.ToLower();
            var falseKeywords = new[] { "0", "-1", "desactivated", "deactivated", "disabled", "error", "failed", "false", "fail", "none", "not", "no", "time out", "timeout", "out", "unsuccess", "unexpected", "unable", "warning" };

            return falseKeywords.Where(valueInLower.Contains).Any(keyword => !valueInLower.Contains(code0)) ? "0" : "1";
        }

        private static void CreateBatch(IReadOnlyCollection<LogStatItem> records)
        {
            if (records == null)
            {
                ChangeThreadCount(-1);
                return;
            }
            ALogger.LogInfo("CreateBatch: start create batch {0}", records.Count);

            var batchCount = 0;
            var totaltestcases = records.Count();

            foreach (var item in records)
            {
                try
                {
                    if ((batchCount % _flushqueueLimit) == 0)
                    {
                        ALogger.LogDebug("CreateBatch: processing item: {0} / {1}", batchCount, totaltestcases);
                    }                    

                        while (FlushQueueReady() || !_mre.WaitOne(_checkFileTimeoutMs))
                        {
                            if (!FlushQueueReady())
                            {
                                ALogger.LogTrace("CreateBatch: Queue is not ready wait for the next {0} ms", _checkFileTimeoutMs);
                                continue;
                            }

                            _flushQueue.EnQueue(item);
                            batchCount++;
                            break;
                        }
                }
                catch (Exception ex)
                {
                    ALogger.LogError("CreateBatch: error processing item: {0}", ex.Message);
                }
            }

            ALogger.LogInfo("CreateBatch: Done create batch {0}/{1}", batchCount, totaltestcases);
            ChangeThreadCount(-1);
        }

        private static string GuidValidator(string guid)
        {
            if (!string.IsNullOrEmpty(guid))
            {
                guid = guid.Replace("{", string.Empty);
                guid = guid.Replace("}", string.Empty);
                if (_guidValidator.IsMatch(guid))
                    return guid;                
            }            
            ALogger.LogWarn("GuidValidator: invalid guid {0}", guid);
            return zeroGuid;            
        }

        private static bool IsEWM(TestResult item)
        {
            return item.ProdName.Equals("Thomson Reuters Eikon for Wealth Management",StringComparison.InvariantCultureIgnoreCase) && 
                   item.ProdVers.Trim().Equals("1.0",StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool LogAppHits(List<LogStatItem> stats)
        {
            using (var appHitsClient = new AppHitsClient(RouterBindings.Local,RouterAddresses.Local.RequestReply))
            {
                var req = new Request
                {
                    TypeHeader = TypeHeader.LOG_HITS,
                    LogHitsReq = new LogHitsReq
                    {
                        Product = "ekn",
                        SubProduct = "app",
                        AppHitItems = new AppHitItems()
                    }
                };

                foreach (var uuid in stats.Where(x => x != null).GroupBy(u => u.uuid))
                {
                    req.LogHitsReq.AppHitItems.Add(new AppHitItem
                    {
                        Uuid = uuid.Key,
                        Region = "",
                        AppName = "SystemTest",
                        IsEmpApp = false,
                        FeatureName = "MTR", // Migrate Test Result
                        BusinessDate = "",
                        GmtTime = DateTime.UtcNow.ToString(),
                        Count = 1
                    });
                    req.LogHitsReq.AppHitItems.Add(new AppHitItem
                    {
                        Uuid = uuid.Key,
                        Region = "",
                        AppName = "SystemTest",
                        IsEmpApp = false,
                        FeatureName = "MTRStat",
                        BusinessDate = "",
                        GmtTime = DateTime.UtcNow.ToString(),
                        Count = uuid.Count()
                    });
                }

                var resp = appHitsClient.Process(req);
                if (resp != null && resp.AppHitResult.ResultCode == 1) // success
                {
                    ALogger.LogInfo("LogAppHits: [{0}] migrate test result feature hit", req.LogHitsReq.AppHitItems.Count);
                    return true;
                }
                else
                {
                    ALogger.LogWarn("LogAppHits: [{0}] cannot send apphit", req.LogHitsReq.AppHitItems.Count);
                    return false;
                }
            }
        }

        private static bool OpsConsoleServiceReady(OpsConsoleServiceClient client)
        {
            var serviceState = client.GetFlipQueueState(new FlipQueueStateRequest());
            return serviceState.ready;
        }

        private static void FlushQueueToOpsConsole(object sender, FlushQueueThresholdBreachedEventArgs<LogStatItem> e)
        {
            var batch = e.ItemsToFlush;
            
            if (!batch.Any())
            {
                return;
            }

            ALogger.LogInfo("FlushQueueToOpsConsole: There are {0} LogStatItems will be sent to OpsConsoleService.", batch.Count);

            try
            {
                using (var client = new OpsConsoleServiceClient(RouterBindings.Local, RouterAddresses.Local.RequestReply))
                {
                    while (OpsConsoleServiceReady(client) || !_mre2.WaitOne(_checkFileTimeoutMs))
                    {
                        if (!OpsConsoleServiceReady(client))
                        {
                            ALogger.LogWarn("FlushQueueToOpsConsole: OpsConsoleService is not ready - will retry later in next time.");
                            continue;
                        }
                        
                        var response = client.LogStats(new LogStatsRequest { stats = batch.ToArray<LogStatItem>() });
                        if (response.success)
                        {
                            ALogger.LogInfo("FlushQueueToOpsConsole: Successfully sent {0} LogStatItems to OpsConsoleService.", batch.Count);
                            LogAppHits(batch.ToList<LogStatItem>());
                        }
                        else
                        {
                            ALogger.LogError("FlushQueueToOpsConsole: LogStats request responded with Failure status.");
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                ALogger.LogError("FlushQueueToOpsConsole: Error sending queue to OpsConsoleService: " + ex.Message);
            }
            
        }

        private static bool FlushQueueReady()
        {
            if (_flushQueue == null)
            {
                return false;
            }

            return _flushQueue.Count <= _flushqueueLimit;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _mre.Set();
                _mre.Dispose();
                _mre2.Set();
                _mre2.Dispose();
                _disposed = true;
            }
        }
        }
    }
