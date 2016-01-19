using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using Newtonsoft.Json;
using ThomsonReuters.Eikon.SystemTest.AppHitsService;
using ThomsonReuters.Eikon.SystemTest.DataCollectorService;
using ThomsonReuters.Eikon.SystemTest.OpsConsoleService;
using ThomsonReuters.Eikon.SystemTestApp;
using TR.AppServer.Common.Interfaces;
using Wcf.Routing;

namespace ThomsonReuters.Eikon.SystemTest.Document
{    
    public class TestResultForwarder
    {
        public const string RequestAnalyzerServiceXmlns = "http://www.reuters.com/RST/TRR/Service/TestResultReceiverService";
        public const string DefaultXmlns = "http://www.reuters.com/rst";
        public const string ProductName = "est";
        public const string ZeroGuid = "00000000-0000-0000-0000-000000000000";
        private const string TimestampFormat = "yyyy-MM-ddTHH:mm:ss.000Z";

        private const string Test127Mspretxiii = "127MSPRETXIII";
        private const string Test127MspretxiiiModified = "127MS1";
        private const string Test127MSPRETXIV = "127MSPRETXIV";
        private const string Test127MSPRETXIVModified = "127MS2";
        private const string Test127MSSBXIII = "127MSSBXIII";
        private const string Test127MSSBXIIIModified = "127MS3";
        private const string Test127MSPINSXIV = "127MSPINSXIV";
        private const string Test127MSPINSXIVModified = "127MS4";

        private static readonly string[] Ordinals = { "ms", "", "k", "m", "g", "t" };

        public string UUID { get; set; }
        public string TargetNamespace { get; private set; }
        public XmlNamespaceManager NamespaceMgr { get; private set; }
        public string XMLSchema { get; private set; }
        public ILogger Logger { get; set; }

        private XmlDocument _mXMLResult = null;
        public XmlDocument XMLResult
        {
            get { return _mXMLResult; }
            set
            {
                if (value.DocumentElement.NamespaceURI.Equals(TargetNamespace))
                {
                    _mXMLResult = value;
                    NamespaceMgr = new XmlNamespaceManager(_mXMLResult.NameTable);
                    NamespaceMgr.AddNamespace(TRRConstant.ElementPrefix, TargetNamespace);
                }
                else
                {                    
                    throw new Exception("This XML data does not have a namespace or invalid namespace");
                }
            }
        }

        private string _productVersion = "";
        private static Metadata _metadata;
        public static Metadata Metadata
        {
            get { return _metadata; }
        }

        public TestResultForwarder()
        {
            if (string.IsNullOrEmpty(XMLSchema))
            {
                using (var sStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ThomsonReuters.Eikon.SystemTest.TestResultXMLSchema.xsd"))
                {
                    using (var reader = new StreamReader(sStream))
                    {
                        sStream.Seek(0, SeekOrigin.Begin);
                        XMLSchema = reader.ReadToEnd();
                    }
                }
            }
            
            TargetNamespace = DefaultXmlns;
        }

        internal bool ForwardTest(string uuid, out string testID)
        {
            testID = "";
            var parser = new TestResultParser();
            UUID = uuid;
            if (!parser.ValidXmlDoc(XMLResult, TargetNamespace, XMLSchema))
            {
                Logger.LogError("This test result from {0} is invalid [{1}]", uuid, parser.ValidationError);
                return false;
            }

            var timeStamp = DateTime.Now.ToString(TimestampFormat);
            var machineGUID = GetValueFromXML(string.Concat("//", TRRConstant.ElementPrefix, ":machineID"));

            try
            {
                testID = GetTestID(machineGUID, timeStamp);
            }
            catch (Exception e)
            {
                Logger.LogError("[{0}] Cannot get TestID from OpsConsole - Exception: {1}", uuid, e.Message);
                return false;
            }
            

            if (string.IsNullOrEmpty(machineGUID))
                machineGUID = ZeroGuid;
            var testResultArray = new[]{new TestResult
            {
                Product = ProductName,
                TimeStamp = timeStamp,
                UUID = uuid,
                InstallGuid = ZeroGuid,
                MachineGUID = machineGUID,
                Stats = GetTestCaseListFromXML(string.Concat("//", TRRConstant.ElementPrefix, ":testCases//", TRRConstant.ElementPrefix, ":testCase"))
            }};

            var product = TestResultForwarder.ProductName;
            _productVersion = GetValueFromXML(string.Concat("//", TRRConstant.ElementPrefix, ":product"));
            var productName = GetValueFromXML(string.Concat("//", TRRConstant.ElementPrefix, ":product/@name"));
            var productVersion = GetValueFromXML(string.Concat("//", TRRConstant.ElementPrefix, ":product/@version"));
            if (IsEWM(productName, productVersion))
            {                             
                Logger.LogInfo("ForwardTest: EWM Mapping {0}: {1}-{2}", uuid, productName, productVersion);
                product = "ewm";
                MapStatCodeEWM(testResultArray[0]);
            }               
            var runningMode = GetValueFromXML(string.Concat("//", TRRConstant.ElementPrefix, ":runningMode"));
            var xnav = XMLResult.CreateNavigator();
            var total = Convert.ToInt32(GetXPathScalar(xnav, "count(/rst:rstResult/rst:testCases/rst:testCase)", NamespaceMgr));
            var totalPass = Convert.ToInt32(GetXPathScalar(xnav, "count(/rst:rstResult/rst:testCases/rst:testCase[not(@shouldBeRerun!='false') and not(@ssiID!='') and rst:valid='pass'])", NamespaceMgr));
            var totalInfo = Convert.ToInt32(GetXPathScalar(xnav, "count(/rst:rstResult/rst:testCases/rst:testCase[not(@shouldBeRerun!='false') and not(@ssiID!='') and rst:valid='info'])", NamespaceMgr));
            var totalWarning = Convert.ToInt32(GetXPathScalar(xnav, "count(/rst:rstResult/rst:testCases/rst:testCase[not(@shouldBeRerun!='false') and not(@ssiID!='') and rst:valid='warning'])", NamespaceMgr));
            var totalAlert = Convert.ToInt32(GetXPathScalar(xnav, "count(/rst:rstResult/rst:testCases/rst:testCase[not(@shouldBeRerun!='false') and not(@ssiID!='') and rst:valid='alert'])", NamespaceMgr));
            var totalFail = Convert.ToInt32(GetXPathScalar(xnav, "count(/rst:rstResult/rst:testCases/rst:testCase[not(@shouldBeRerun!='false') and not(@ssiID!='') and rst:valid='fail'])", NamespaceMgr));
            var other = total - totalPass - totalInfo - totalWarning - totalAlert - totalFail;            

            testResultArray[0].Stats.Add(new TestCase { StatName = "runmode", StatVal = runningMode });
            testResultArray[0].Stats.Add(new TestCase { StatName = "total", StatVal = total.ToString() });
            testResultArray[0].Stats.Add(new TestCase { StatName = "pass", StatVal = totalPass.ToString() });
            testResultArray[0].Stats.Add(new TestCase { StatName = "info", StatVal = totalInfo.ToString() });
            AddValidationStat(xnav, testResultArray[0], "warn", totalWarning);
            AddValidationStat(xnav, testResultArray[0], "alert", totalAlert);
            AddValidationStat(xnav, testResultArray[0], "fail", totalFail);
            testResultArray[0].Stats.Add(new TestCase { StatName = "other", StatVal = other.ToString() });

            var compName = Environment.GetEnvironmentVariable("COMPUTERNAME") ?? "";
            var envName = Environment.GetEnvironmentVariable("AS_ENV") ?? "";

            testResultArray[0].Stats.Add(new TestCase { StatName = "servname", StatVal = compName });
            testResultArray[0].Stats.Add(new TestCase { StatName = "envname", StatVal = envName });
            testResultArray[0].Stats.Add(new TestCase { StatName = "testID", StatVal = testID });
            testResultArray[0].Stats.Add(new TestCase { StatName = "product", StatVal = product });

            var json = JsonConvert.SerializeObject(testResultArray);

            // TODO: comment out after debuging
            /*var fileName = string.Format("{0}-{1:yyyyMMdd}.json", uuid, DateTime.Now);
            var offlineResultsFolderPath = Path.Combine(Environment.ExpandEnvironmentVariables("%AS_DATA%"), "RSTStat\\OfflineResults\\");
            if (!Directory.Exists(offlineResultsFolderPath))
            {
                Directory.CreateDirectory(offlineResultsFolderPath);
            }
            File.WriteAllText(Path.Combine(offlineResultsFolderPath, fileName), json);
            */
            var compressedBytes = new MemoryStream();
            var rawBytes = new MemoryStream(Encoding.UTF8.GetBytes(json)); // auto disposed by GzipStream
            using (var gzip = new GZipStream(compressedBytes, CompressionMode.Compress, leaveOpen: true))
            {
                rawBytes.CopyTo(gzip);
                gzip.Flush();
            }
            compressedBytes.Position = 0L;
             Task.Run(() =>
                {
                    try
                    {
                        using (
                            var client = new PlatformUsageDataCollectorClient(RouterBindings.Local,
                                RouterAddresses.Local.RequestReply))
                        {
                            client.WriteOpsConsoleDesktopMessageGzip(compressedBytes);
                            Logger.LogInfo("ForwardTest: [{0}] test result forwarded", UUID);
                            using (
                                var appHitsClient = new AppHitsClient(RouterBindings.Local,
                                    RouterAddresses.Local.RequestReply))
                            {
                                AppHitsService.Response resp = null;
                                var appHitItems = 

                                resp = appHitsClient.Process(new Request
                                {
                                    TypeHeader = TypeHeader.LOG_HITS,
                                    LogHitsReq = new LogHitsReq
                                    {
                                        Product = "ekn",
                                        SubProduct = "app",
                                        AppHitItems = new AppHitItems
                                        {
                                            new AppHitItem
                                            {
                                                Uuid = uuid,
                                                Region = "",
                                                AppName = "SystemTest",
                                                IsEmpApp = false,
                                                FeatureName = "STR", // SubmitTestResult
                                                BusinessDate = "",
                                                GmtTime = timeStamp,
                                                Count = 1
                                            }
                                        }
                                    }
                                });
                                if (resp != null && resp.AppHitResult.ResultCode == 1) // success
                                {
                                    Logger.LogInfo("ForwardTest: [{0}] submit test result feature hit", UUID);
                                }
                                else
                                {
                                    Logger.LogWarn("ForwardTest: [{0}] cannot send apphit", UUID);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError("ForwardTest: [{0}], {1}", UUID, ex.Message);
                    }
                });
             return true;
        }

        public void AddValidationStat(XPathNavigator xpathnav, TestResult testResult, string valid, int total)
        {
            var validMsg = valid == "warn" ? "warning" : valid;
            var validationIDList = GetTestCaseIDListByValidation(xpathnav, validMsg, NamespaceMgr);
            var i = 0;
            if (validationIDList.Count == 0 || total == 0)
            {
                testResult.Stats.Add(new TestCase { StatName = valid, StatVal = "0" });
                return;
            }
            foreach (var varidateIds in validationIDList)
            {
                if (i == 0)
                {
                    if (i == validationIDList.Count-1)
                        testResult.Stats.Add(new TestCase { StatName = valid, StatVal = total + "[" + varidateIds + "]" });
                    else
                        testResult.Stats.Add(new TestCase { StatName = valid, StatVal = total + "[" + varidateIds });
                }
                else
                {
                    if (i == validationIDList.Count-1)
                        testResult.Stats.Add(new TestCase { StatName = valid + "." + i, StatVal = varidateIds + "]" });
                    else
                        testResult.Stats.Add(new TestCase { StatName = valid + "." + i, StatVal = varidateIds });
                }
                i++;
            }
        }

        public void MapStatCodeEWM(TestResult testResult)
        {
            foreach (var testCase in testResult.Stats)
            {
                if (_metadata == null)
                {
                    _metadata = GetMetadata();
                }

                var mapCode = _metadata.StatCodeDict.Find(s => s.TestName.Contains(testCase.StatName));
                if (mapCode == null) continue;
                foreach (var map in mapCode.Map)
                {
                    testCase.StatName = map.ID;
                }
            }
        }

        public string GetValueFromXML(string xpath)
        {
            var valueInXML = "";
            try
            {
                var selectedNode = XMLResult.SelectSingleNode(xpath, NamespaceMgr);
                if (selectedNode != null)
                {
                    valueInXML = selectedNode.InnerText;
                }
            }
            catch (XPathException ex)
            {
                Logger.LogError("GetValueFromXML: [{0}], {1}", UUID, ex.Message);
            }

            return valueInXML;
        }

        public static bool IsEWM(string pName, string pVer)
        {
            return pName.Equals("Thomson Reuters Eikon for Wealth Management", StringComparison.InvariantCultureIgnoreCase) && 
                   pVer.Trim().Equals("1.0", StringComparison.InvariantCultureIgnoreCase);
        }

        public List<TestCase> GetTestCaseListFromXML(string xpath)
        {
            var testCaseList = new List<TestCase>();
            XmlNodeList testCases = null;
            try
            {
                testCases = XMLResult.SelectNodes(xpath, NamespaceMgr);
            }
            catch (XPathException)
            {
                return null;
            }

            foreach (XmlNode testcase in testCases)
            {
                if (testcase.Attributes == null) continue;

                var name = testcase.Attributes["id"].Value.Replace("#test.", "");
                if (name == "24")
                {
                    name = "CMPNAME";
                }
                var value = "";
                var desc = "";
                var valueNode = testcase.SelectSingleNode("rst:value", NamespaceMgr);
                var descNode = testcase.SelectSingleNode("rst:description", NamespaceMgr);
                if (valueNode == null)
                    value = "n/a";
                else
                {
                    valueNode = valueNode.LastChild;
                    if (valueNode == null)
                        value = "n/a";
                    else
                        value = valueNode.Value;
                }

                if (descNode == null)
                    desc = "n/a";
                else
                {
                    descNode = descNode.LastChild;
                    if (descNode == null)
                        desc = "n/a";
                    else
                        desc = descNode.Value;
                }
                                    
                if (name == "127")
                {
                    const string pattern = @"([\D^.,]*)([\d|.]*),";
                    var rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                    var matches = rgx.Matches(value);

                    testCaseList.AddRange(from Match match in matches where match.Success 
                                            let testName = match.Groups[1].ToString() 
                                            let testValue = match.Groups[2].ToString()
                                          select CreateTestCaseInstance(ShortenTest127LongName(name+testName), testValue, desc) into testCase 
                                            where testCase != null select testCase);
                }
                else if (name == "142")
                {
                    const string pattern = @"\{(Primary Screen=(\w+),\{x=(-*\d+),y=(-*\d+),width=(-*\d+),height=(-*\d+)\})\}+";
                    var rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                    var matches = rgx.Matches(value);

                    var i = 0;
                    testCaseList.AddRange(from Match match in matches where match.Success let newName = String.Format("{0}.{1}", name, ++i)
                                          let newValue = String.Format("{0}{1},{2},{3},{4}", match.Groups[2].Value.Equals("True", StringComparison.CurrentCultureIgnoreCase) ? "Primary " : "", 
                                            match.Groups[3].Value, match.Groups[4].Value, match.Groups[5].Value, match.Groups[6].Value)
                                          select CreateTestCaseInstance(newName, newValue, desc) into testCase 
                                          where testCase != null select testCase);
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
                    var testCase = CreateTestCaseInstance(name.Truncate(10), value, desc);
                    if (testCase != null)
                        testCaseList.Add(testCase);
                }
            }

            return testCaseList;
        }        

        private bool IsEikon4Stat()
        {
            return _productVersion.StartsWith("4");
        }

        private TestCase CreateTestCaseInstance(string name, string value, string description)
        {
            var testCase = new TestCase { StatName = name, StatVal = value };
            if (_metadata == null)
            {
                _metadata = GetMetadata();
            }

            var index = _metadata.Testschema.FindIndex(s => s.Name == name);
            if (index == -1)
            {
                Logger.LogWarn("Cannot find metadata for [{0}]", name);
                return null;
            }                
            var schema = _metadata.Testschema[index];
            if (!schema.Enabled)
            {
                Logger.LogWarn("metadata not enable for [{0}]", name);
                return null;
            }
            
            if (schema.Usedesc || string.IsNullOrWhiteSpace(value) || value.Equals("n/a"))
            {
                if (!string.IsNullOrWhiteSpace(description) && description != "-")
                {
                    testCase.StatVal = description;
                }
            }

            if (schema.Type == "boolean")
            {
                testCase.StatVal = ParseBoolean(testCase.StatVal);
                return testCase;
            }

            if (!string.IsNullOrEmpty(schema.Unit))
            {
                testCase.StatVal = ParseUnit(testCase.StatVal, schema.Unit);
                return testCase;
            }

            if (testCase.StatVal.Length <= 40) return testCase;
           
            //var statVal = testCase.StatVal;
            var mapCode = _metadata.EnumDict.Find(s => s.Test.Contains(name));
            if (mapCode != null)
            {
                foreach (var map in mapCode.Map)
                {                    
                    testCase.StatVal = ReplaceCaseInsensitive(testCase.StatVal, map.Value, "#" + map.ID, map.Display==null);
                }
            }
            
            if (testCase.StatVal.Length <= 40) return testCase;
            Logger.LogWarn("[{0}] value length is over 40 charecter, [{1}]", testCase.StatName, testCase.StatVal);
            testCase.StatVal = testCase.StatVal.Substring(0, 37) + "...";

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

        private Metadata GetMetadata(string product="est")
        {
            try
            {
                using (var client = new OpsConsoleServiceClient(RouterBindings.Local, RouterAddresses.Local.RequestReply))
                {
                    var request = new GetMetadataRequest()
                    {
                        product = product
                    };
                    var metadataJson = client.GetMetadata(request);

                    return JsonConvert.DeserializeObject<Metadata>(metadataJson);
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarn("GetMetadata: [{0}], cannot get metadata from OpsConsole service, {1}", UUID, ex.Message);
                try
                {
                    using (var sStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ThomsonReuters.Eikon.SystemTest.Schema.json"))
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
                    Logger.LogError("GetMetadata: [{0}], cannot get metadata from resource file, {1}", UUID, ex2.Message);
                    return null;
                }
            }
        }

        private string GetTestID(string machineGuid, string timeStamp, string product = "est")
        {          
            using (var client = new OpsConsoleServiceClient(RouterBindings.Local, RouterAddresses.Local.RequestReply))
            {
                var request = new GetTestIDRequest()
                {
                    machineGuid = machineGuid,
                    uuid = UUID,
                    timeStamp = timeStamp,
                    product = product
                };
                var testID = client.GetTestID(request);

                return testID.ToString();
            }            
        }

        private string ShortenTest127LongName(string testName)
        {
            if (testName.Length <= 10)
            {
                return testName;
            }

            switch (testName)
            {
                case Test127Mspretxiii:
                    return Test127MspretxiiiModified;
                case Test127MSPRETXIV:
                    return Test127MSPRETXIVModified;
                case Test127MSSBXIII:
                    return Test127MSSBXIIIModified;
                case Test127MSPINSXIV:
                    return Test127MSPINSXIVModified;
            }
            Logger.LogWarn("ShortenTest127LongName: Long name cannot be modified: {0}", testName);
            return testName.Truncate(10);
        }      

        private string ParseUnit(string value, string schemaUnit)
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
                Logger.LogWarn("ParseUnit: [{0}!{1}], {2}",value, schemaUnit,  ex.Message);
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
            var falseKeywords = new[] { "0", "-1", "desactivated", "deactivated", "disabled", "error", "failed", "false", "fail", "none", "not", "no", "time out", "timeout", "out", "unsuccess", "unexpected", "unable", "warning", "n/a" };

            return falseKeywords.Where(valueInLower.Contains).Any(keyword => !valueInLower.Contains(code0)) ? "0" : "1";
        }

        public object GetXPathScalar(XPathNavigator xpathnav, string expression, XmlNamespaceManager nsmgr)
        {
            if (xpathnav == null)
            {
                return null;
            }
            var expr = xpathnav.Compile(expression);
            expr.SetContext(nsmgr);

            var result = xpathnav.Evaluate(expr);
            switch (expr.ReturnType)
            {
                case XPathResultType.Number:
                    if (result == null)
                    {
                        return 0;
                    }
                    else
                    {
                        return (double)result;
                    }

                case XPathResultType.Boolean:
                    if (result == null)
                    {
                        return false;
                    }
                    else
                    {
                        return (bool)result;
                    }

                case XPathResultType.String:
                    if (result == null)
                    {
                        return "";
                    }
                    else
                    {
                        return (string)result;
                    }

                case XPathResultType.NodeSet:
                    return xpathnav.Select(expression, nsmgr);

                default:
                    return result;
            }
        }

        public List<string> GetTestCaseIDListByValidation(XPathNavigator xpathnav, string valid, XmlNamespaceManager nsmgr)
        {
            List<string> testCaseIdList = new List<string>();
            var expression = "/rst:rstResult/rst:testCases/rst:testCase[not(@shouldBeRerun!='false') and not(@ssiID!='') and rst:valid='" + valid + "']";
            var testCaseIds = new StringBuilder();
            if (xpathnav == null)
            {
                return null;
            }
            var iterator = xpathnav.Select(expression, nsmgr);            
            while (iterator.MoveNext())
            {
                if (testCaseIds.Length > 0 && testCaseIds[testCaseIds.Length-1] != ',')
                    testCaseIds.Append(",");
                var name = iterator.Current.GetAttribute("id", "").Replace("#test.", "");
                if (name == "24")
                {
                    name = "CMPNAME";
                }
                testCaseIds.Append(name);
                if (testCaseIds.Length >= 30 || (iterator.CurrentPosition == iterator.Count))
                {
                    testCaseIdList.Add(testCaseIds.ToString());
                    testCaseIds.Clear();
                    testCaseIds.Append(",");
                }
            }
            return testCaseIdList;
        }
    }

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
}
