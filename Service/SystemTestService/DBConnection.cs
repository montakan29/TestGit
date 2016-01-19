using System;
using System.Data.SqlClient;
using SystemTestService.Utility;
using TR.AppServer.Common.Interfaces;
using TR.AppServer.Logging;

namespace SystemTestService
{
    internal class DBConnection
    {
        private static readonly ILogger ALogger = Logger.Default;
        private static readonly string ConString = ConfigUtil.DBConnStr;

        internal static SqlConnection CreateSqlCon()
        {
            SqlConnection con;
            var conString = "";
            try
            {
                conString = String.Format(ConString, ConfigUtil.GetDBHost(), ConfigUtil.RSTDBUser, ConfigUtil.RSTDBPassword);
                ALogger.LogInfo("Create a sql connection to RST DB from:{0} using: [{1}]", Environment.MachineName, conString);
                con = new SqlConnection(conString);

                con.Open();
            }
            catch (Exception e)
            {
                con = null;
                ALogger.LogError("Failed to create a sql connection to RST DB from:{0} using: [{1}] with the exception msg: {2}", Environment.MachineName, conString, e.Message);
                ALogger.LogException(e);
            }

            return con;
        }
    }

    internal class OpsConsoleDB
    {
        private const string OpsConsoleDBPrefix = "Ops";
        private static readonly ILogger ALogger = Logger.Default;

        internal static SqlConnection CreateSqlCon()
        {
            SqlConnection con;
            var conString = "";
            try
            {              
                conString = ConfigUtil.GetAppDBConnStr(OpsConsoleDBPrefix);
                //ALogger.LogInfo("Create a sql connection to OpsConsole DB from: {0} using: [{1}]", Environment.MachineName, conString);
                con = new SqlConnection(conString);

                con.Open();
            }
            catch (Exception e)
            {
                con = null;
                ALogger.LogError("Failed to create a sql connection to OpsConsole DB from: {0} using: [{1}] with the exception msg: {2}", Environment.MachineName, conString, e.Message);
                ALogger.LogException(e);
            }

            return con;
        }
    }
}
