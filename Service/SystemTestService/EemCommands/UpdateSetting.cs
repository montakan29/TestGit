using EikonEnvManager.ProcessManagement;
using SystemTestService.Utility;
using TR.AppServer.Common.Interfaces;
using TR.AppServer.Logging;

namespace SystemTestService.EemCommands
{
    class UpdateSetting : CommandBase
    {
        private static readonly ILogger _Logger = Logger.Default;

        [CmdParam("SettingName", DefaultValue = "", Required = false, Description = "Setting Name")]
        public string SettingName { get; set; }

        [CmdParam("SettingValue", DefaultValue = "", Required = false, Description = "Setting Value")]
        public string SettingValue { get; set; }

        /// <summary>
        /// This command will toggle data source.
        /// </summary>
        public UpdateSetting()
            : base("UpdateSetting", "recordflag/useopsdb/dumpopsstat/dumpststat/lastrun/ftp2bi/dumpint/dumpdelay/threadlimit")
        {
        }

        public override CmdResult Execute()
        {
            var settingName = SettingName.ToLowerInvariant();
            var msg = "";
            switch (settingName)
            {
                case "recordflag":
                    if (SettingValue == "")
                    {
                        msg = "Setting: " + settingName + "  is  " + ConfigUtil.DumpNumberOfRecord;
                    }
                    ConfigUtil.DumpNumberOfRecord = SettingValue.ToLowerInvariant()=="true"? true :false;
                    msg = "Setting: " + settingName + "  is set to " + ConfigUtil.DumpNumberOfRecord;
                    break;
                case "useopsdb":
                    ConfigUtil.UseOpsConsoleDBFlag = SettingValue.ToLowerInvariant() == "true" ? true : false;
                    msg = "Setting: " + settingName + "  is set to " + ConfigUtil.UseOpsConsoleDBFlag;
                    break;
                case "dumpopsstat":
                    ConfigUtil.DumpOpsStatFlag = SettingValue.ToLowerInvariant() == "true" ? true : false;
                    msg = "Setting: " + settingName + "  is set to " + ConfigUtil.DumpOpsStatFlag;
                    break;
                case "dumpststat":
                    ConfigUtil.DumpSTStatFlag = SettingValue.ToLowerInvariant() == "true" ? true : false;
                    msg = "Setting: " + settingName + "  is set to " + ConfigUtil.DumpSTStatFlag;
                    break;
                case "lastrun":
                    ConfigUtil.SetDBConfig("LastSTRun", SettingValue.ToLowerInvariant(), "Last Success Run of ST Data Dump to BI");
                    msg = "Setting: " + settingName + "  is set to " + ConfigUtil.GetDBConfig("LastSTRun");
                    break;
                case "ftp2bi":
                    ConfigUtil.Ftp2Bi = SettingValue.ToLowerInvariant() == "true" ? true : false;
                    msg = "Setting: " + settingName + "  is set to " + ConfigUtil.Ftp2Bi;
                    break;
                case "dumpint":
                    ConfigUtil.JobDumpIntervalMinute = System.Convert.ToInt32(SettingValue);
                    msg = "Setting: " + settingName + "  is set to " + ConfigUtil.JobDumpIntervalMinute;
                    break;
                case "dumpdelay":
                    ConfigUtil.JobDumpDelayMinute = System.Convert.ToInt32(SettingValue);
                    msg = "Setting: " + settingName + "  is set to " + ConfigUtil.JobDumpDelayMinute;
                    break;
                case "threadlimit":
                    ConfigUtil.ThreadLimit = System.Convert.ToInt32(SettingValue);
                    msg = "Setting: " + settingName + "  is set to " + ConfigUtil.ThreadLimit;
                    break;
                default:
                    msg = "no setting matched ";
                    msg += "/recordflag " + ConfigUtil.DumpNumberOfRecord;
                    msg += "/useopsdb " + ConfigUtil.UseOpsConsoleDBFlag;
                    msg += "/dumpopsstat " + ConfigUtil.DumpOpsStatFlag;
                    msg += "/dumpststat " + ConfigUtil.DumpSTStatFlag;
                    msg += "/lastrun " + ConfigUtil.GetDBConfig("LastSTRun");
                    msg += "/ftp2bi " + ConfigUtil.Ftp2Bi;
                    msg += "/dumpint " + ConfigUtil.JobDumpIntervalMinute.ToString();
                    msg += "/dumpdelay " + ConfigUtil.JobDumpDelayMinute.ToString();
                    msg += "/threadlimit " + ConfigUtil.ThreadLimit.ToString();
                    break;
            }

            
            _Logger.LogInfo(msg);
            return CmdResult.Success(msg);
        }
    }
}
