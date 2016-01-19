using EikonEnvManager.ProcessManagement;
using TR.AppServer.Common.Interfaces;
using TR.AppServer.Logging;

namespace SystemTestService.EemCommands
{
    class MergeTestID : CommandBase
    {
        private static readonly ILogger _Logger = Logger.Default;
        public MergeTestID()
            : base("Merge SystemTestID", "Merge SystemTest ID")
        {
        }

        public override CmdResult Execute()
        {           
            _Logger.LogInfo("Merging stat to OpsConsole ...");
            DataDumper.MergeOpsConsoleTestID();
            return CmdResult.Success("Please find results from service log");       
        }
    }
}
