using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TR.AppServer.Common.Interfaces;
using TR.AppServer.Logging;
using SystemTestService.OpsConsoleService;
using SystemTestService.Entities;
using Wcf.Routing;

namespace SystemTestService
{
    public class StatCode
    {
        public string Product { get; set; }
        public string Stat { get; set; }
    }
    public class DataMapper
    {
        private static Metadata _estMetaData;
        private static readonly ILogger Logger = TR.AppServer.Logging.Logger.Default;

        public static List<Stat> MapDumpStatsValue(List<Stat> stats)
        {
            try
            {
                if (stats.Count != 0)
                {
                    foreach (var stat in stats.Where(NeedValueMapping))
                    {
                        MapValue(stat);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarn("MapGetStatResponseValue: exception {0}", ex.Message);
            }
            return stats;
        }

        private static void MapValue(Stat stat)
        {
            stat.StatValue = MapValue(stat.StatCode, stat.StatValue);
            return;
        }

        private static string MapValue(string statCode, string statValue)
        {
            if (!NeedValueMapping(statValue)) return statValue;
            var mapCode = GetMetadata().enumdict.Find(s => s.Test.Contains(statCode));
            if (mapCode == null) return statValue;
            foreach (var map in mapCode.Map)
            {
                statValue = ReplaceCaseInsensitive(statValue, "#" + map.ID, map.Value, map.Display == null);
                if (!NeedValueMapping(statValue)) break;
            }
            return statValue;
        }

        internal static string MapStatCode(string statCode)
        {
            if (statCode == "CMPNAME")
            {
                statCode = "24";
            }            

            var testCaseID = "#test." + statCode;
            return testCaseID;
        }

        public static void MapStatCodeEWM(TestResult testResult)
        {
            foreach (var testCase in testResult.TestCases)
            {
                var mapCode = GetMetadata().statcodedict.Find(s => s.TestName.Contains(testCase.Title));
                if (mapCode == null) continue;
                foreach (var map in mapCode.Map)
                {
                    testCase.ID = map.ID;                    
                }
            }
        }

        private static Metadata GetMetadata(string product = "est")
        {
            if (_estMetaData != null) return _estMetaData;
            try
            {
                _estMetaData = new Metadata { enumdict = new List<EnumDict>(), statcodedict = new List<StatCodeDict>() };
                using (var client = new OpsConsoleServiceClient(RouterBindings.Local, RouterAddresses.Local.RequestReply))
                {
                    var request = new GetMetadataRequest()
                    {
                        product = product
                    };
                    var metadataJson = client.GetMetadata(request);
                    _estMetaData = JsonConvert.DeserializeObject<Metadata>(metadataJson);
                }                
            }
            catch (Exception ex)
            {
                Logger.LogWarn("GetMetadata:, cannot get/convert metadata from OpsConsole service, {0}", ex.Message);

                try
                {
                    using (var sStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SystemTestService.Schema.json"))
                    {
                        using (var reader = new StreamReader(sStream))
                        {
                            sStream.Seek(0, SeekOrigin.Begin);
                            var metadataJson = reader.ReadToEnd();
                            _estMetaData = JsonConvert.DeserializeObject<Metadata>(metadataJson);
                        }
                    }
                }
                catch (Exception ex2)
                {
                    Logger.LogError("GetMetadata: cannot get metadata from resource file, {0}", ex2.Message);
                    return _estMetaData;
                }
            }
            return _estMetaData;
        }

        private static string ReplaceCaseInsensitive(string input, string search, string replacement, bool isEscape = true)
        {
            var result = Regex.Replace(
                input,
                isEscape ? Regex.Escape(search) : search,
                replacement.Replace("$", "$$"),
                RegexOptions.IgnoreCase
            );
            return result;
        }

        private static bool NeedValueMapping(Stat stat)
        {
            return ((stat.Product == "est" || stat.Product == "ewm") && NeedValueMapping(stat.StatValue));
        }

        private static bool NeedValueMapping(string value)
        {
            return value.Contains("#");
        }
    }
}
