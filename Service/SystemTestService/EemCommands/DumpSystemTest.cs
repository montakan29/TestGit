using EikonEnvManager.ProcessManagement;
using System;
using System.IO;
using TR.AppServer.Common.Interfaces;
using TR.AppServer.Logging;

namespace SystemTestService.EemCommands
{
    class DumpSystemTest : CommandBase
    {
        private static readonly ILogger _Logger = Logger.Default;
        readonly string _localSaveFolder = Path.Combine(Environment.ExpandEnvironmentVariables("%AS_DATA%"), "RSTStat","Adhoc");

        [CmdParam("Mode", DefaultValue = "4", Required = false, Description = "Dump Mode")]
        public int Mode { get; set; }

        [CmdParam("Start", DefaultValue = "", Required = false, Description = "Start Date/Time to dump data")]
        public string Start { get; set; }

        [CmdParam("End", DefaultValue = "", Required = false, Description = "End Date/Time to dump data")]
        public string End { get; set; }

        [CmdParam("Limit", DefaultValue = "", Required = false, Description = "# of Record limit, empty for no limit")]
        public string Limit  { get; set; }

        /// <summary>
        /// This command will dump SystemTest stats to OpsConsole like DataDumper.DumpRSTStat does.
        /// </summary>
        public DumpSystemTest()
            : base("Dump SystemTest", "Force System Test to dump: Date Format (YYYY-MM-DD HH:MM:SS), Mode: [1:DumpOnly],[2:Dump+FTP],[3:Dump+Upload],[4:Dump+FTP+Upload]")
        {
        }

        public override CmdResult Execute()
        {
            try {
                var x = 0;
                _Logger.LogInfo("query ST stat from {0} to {1} in mode {2}", Start, End, Mode);
                var dumper = new DataDumper
                {
                    RecordLimit = (!string.IsNullOrEmpty(Limit) && Int32.TryParse(Limit, out x)) ? x : 0
                };
                var result = dumper.DumpRSTStatInterval(_localSaveFolder, Start, End, Mode);
                return result != null ? CmdResult.Success("Dumping SystemTest complete." + result) : CmdResult.Failure("Dumping SystemTest error or no stats");
            }
            catch (Exception e)
            {
                _Logger.LogError("Dumping SystemTest command error" + e.Message);
                _Logger.LogException(e);
                return CmdResult.Failure("Dumping SystemTest command error" + e.Message);
            }
        }
    }
}
