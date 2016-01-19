using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EikonEnvManager.ProcessManagement;
using TR.AppServer.Common.Interfaces;
using TR.AppServer.Logging;

namespace SystemTestService.EemCommands
{
    internal class CleanTempFile : CommandBase
    {
        private readonly ILogger _logger = Logger.Default;
        readonly string _localSaveFolder = Path.Combine(Environment.ExpandEnvironmentVariables("%AS_DATA%"), "RSTStat");

        [CmdParam("Mode", DefaultValue = 1, Required = false, Description = "Delete Mode")]
        public int Mode { get; set; }

        [CmdParam("KeepDay", DefaultValue = 7, Required = false, Description = "Keep Day")]
        public int KeepDay { get; set; }

        [CmdParam("List", DefaultValue = true, Required = false, Description = "Flag to true to list the file to be deleted but not deleted them yet")]
        public bool List { get; set; }

        public CleanTempFile()
            : base("Clean tmp File", "Clean temporary dump files Mode: [1:clean tmp json files],[2:clean regular dump file],[3:clean adhoc dump file], Period: Keep Day, List: list only")
        {
        }

        public override CmdResult Execute()
        {
            var targetFolder = _localSaveFolder;
            var result = "";
            _logger.LogInfo("CleanTempFile: Mode {0}, KeepDay {1}, List Flag {2}", Mode, KeepDay, List);
            switch (Mode)
            {
                case 1:
                    result =DataDumper.CleanUpTempFile(targetFolder, List);
                    break;
                case 2:
                    result = DataDumper.CleanupFile(targetFolder, List, KeepDay);
                    break;
                case 3:
                    targetFolder = Path.Combine(targetFolder, "Adhoc");
                    result = DataDumper.CleanupFile(targetFolder, List, KeepDay);
                    break;
            }
            return CmdResult.Success("Clean tmp file done: " + result);
        }         
    }
}

