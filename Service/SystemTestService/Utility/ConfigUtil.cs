using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using TR.AppServer.Common.Interfaces;
using TR.AppServer.Logging;

namespace SystemTestService.Utility
{
    public static class ConfigUtil
    {
        private static bool _dumpNumberOfRecordFlag = false;
        private static bool _useOpsConsoleDBFlag = true;
        private static bool _dumpOpsStatFlag = true;
        private static bool _dumpSTStatFlag = true;
        private static bool _ftp2bi = false;
        private static int _jobDumpIntervalMinute = Convert.ToInt32(ConfigurationManager.AppSettings["JobDumpIntervalMinute"]);
        private static int _jobDumpDelayMinute = Convert.ToInt32(ConfigurationManager.AppSettings["JobDumpDelayMinute"]);
        private static int _threadLimit = 10;
        private static readonly ILogger ALogger = Logger.Default;
        private const string _DBConfigSettingPrefix = "DumpToBIService_";
        public static bool DumpNumberOfRecord {
            get { return _dumpNumberOfRecordFlag;  }
            set { _dumpNumberOfRecordFlag = value; }
        }

        public static bool UseOpsConsoleDBFlag
        {
            get
            {
                var setting = GetDBConfig("useOpsConsoleDBFlag");
                if (!string.IsNullOrEmpty(setting))
                {
                    _useOpsConsoleDBFlag = Convert.ToBoolean(setting);
                }
                else
                {
                    ALogger.LogInfo("Can't get useOpsConsoleDBFlag setting from DB use default value {0}", _useOpsConsoleDBFlag);
                }
                return _useOpsConsoleDBFlag;
            }
            set
            {
                _useOpsConsoleDBFlag = value;
                SetDBConfig("useOpsConsoleDBFlag", value.ToString(), "Use OpsConsole DB for System Test Dump to BI");
            }
        }

        public static bool DumpOpsStatFlag
        {
            get
            {
                var setting = GetDBConfig("dumpOpsStatFlag");
                if (!string.IsNullOrEmpty(setting))
                {
                    _dumpOpsStatFlag = Convert.ToBoolean(setting);
                }
                else
                {
                    ALogger.LogInfo("Can't get dumpOpsStatFlag setting from DB use default value {0}", _dumpOpsStatFlag);
                }
                return _dumpOpsStatFlag;
            }
            set
            {
                _dumpOpsStatFlag = value;
                SetDBConfig("dumpOpsStatFlag", value.ToString(), "Flag to Dump OpsConsole to BI");
            }

        }

        public static bool DumpSTStatFlag
        {
            get
            {
                var setting = GetDBConfig("dumpSTStatFlag");
                if (!string.IsNullOrEmpty(setting))
                {
                    _dumpSTStatFlag = Convert.ToBoolean(setting);
                }
                else
                {
                    ALogger.LogInfo("Can't get dumpSTStatFlag setting from DB use default value {0}", _dumpSTStatFlag);
                }
                return _dumpSTStatFlag;
            }
            set
            {
                _dumpSTStatFlag = value;
                SetDBConfig("dumpSTStatFlag", value.ToString(), "Flag to Dump System Test to BI");
            }

        }

        public static bool Ftp2Bi
        {
            get
            {
                var setting = GetDBConfig("ftp2bi");
                if (!string.IsNullOrEmpty(setting))
                {
                    _ftp2bi = Convert.ToBoolean(setting);
                }
                else
                {
                    ALogger.LogInfo("Can't get _ftp2bi setting from DB use default value {0}", _ftp2bi);
                }
                return _ftp2bi;
            }
            set
            {
                _ftp2bi = value;
                SetDBConfig("ftp2bi", value.ToString(), "FTP to BI flag");
            }

        }

        public static int JobStartHour
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["JobStartHour"]);
            }
        }
        public static int JobStartMinute
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["JobStartMinute"]);
            }
        }
        public static int IntervalMinute
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["IntervalMinute"]);
            }
        }
        public static int JobDumpIntervalMinute
        {
            get
            {
                var setting = GetDBConfig("JobDumpIntervalMinute");
                if (!string.IsNullOrEmpty(setting))
                {
                    int jobDumpIntervalMinute = 0;
                    int.TryParse(setting, out jobDumpIntervalMinute);
                    if (jobDumpIntervalMinute != 0)
                        _jobDumpIntervalMinute = jobDumpIntervalMinute;
                    else
                        ALogger.LogInfo("Can't get JobDumpIntervalMinute setting from DB use default value {0}", _jobDumpIntervalMinute);
                }
                else
                {
                    ALogger.LogInfo("Can't get JobDumpIntervalMinute setting from DB use default value {0}", _jobDumpIntervalMinute);
                }
                return (_jobDumpIntervalMinute > 1440) ? 1440 : _jobDumpIntervalMinute;
            }
            set
            {
                _jobDumpIntervalMinute = value;
                SetDBConfig("JobDumpIntervalMinute", value.ToString(), "Job Dump Interval Minute");
            }
        }
        public static int JobDumpDelayMinute
        {
            get
            {
                var setting = GetDBConfig("JobDumpDelayMinute");
                if (!string.IsNullOrEmpty(setting))
                {
                    _jobDumpDelayMinute = Convert.ToInt32(setting);
                }
                else
                {
                    ALogger.LogInfo("Can't get JobDumpDelayMinute setting from DB use default value {0}", _jobDumpIntervalMinute);
                }
                return _jobDumpDelayMinute;
            }
            set
            {
                _jobDumpIntervalMinute = value;
                SetDBConfig("JobDumpDelayMinute", value.ToString(), "Job Dump Interval Minute");
            }
        }
        public static int DBQueryTimeout
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["DBQueryTimeout"]);
            }
        }

        public static string DBConnStr
        {
            get
            {
                return ConfigurationManager.AppSettings["DBConnStr"];
            }
        }

        public static string RSTDBUser
        {
            get
            {
                return ConfigurationManager.AppSettings["RSTDBUser"];
            }
        }

        public static string RSTDBPassword
        {
            get
            {
                return ConfigurationManager.AppSettings["RSTDBPassword"];
            }
        }

        public static string EDWFTPUser
        {
            get
            {
                return ConfigurationManager.AppSettings["EDWFTPUser"];
            }
        }

        public static string EDWFTPPassword
        {
            get
            {
                return ConfigurationManager.AppSettings["EDWFTPPassword"];
            }
        }

        public static int EDWFTPPort
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["EDWFTPPort"]);
            }
        }
        
        public static string GetEnv()
        {
            var asEnv = Environment.GetEnvironmentVariable("AS_ENV") ?? "";
            return asEnv;
        }

        public static string GetDBHost()
        {
            var dbHost = "";
            var asEnv = GetEnv();
            switch (asEnv.ToLower())
            {
                case "dev":
                    dbHost = ConfigurationManager.AppSettings["RSTDBDev"];
                    break;
                case "alpha":
                    dbHost = ConfigurationManager.AppSettings["RSTDBAlpha"];
                    break;
                case "ppe1":
                    dbHost = ConfigurationManager.AppSettings["RSTDBBeta"];
                    break;
                case "hdcp":
                case "ntcp":
                case "dtcp":
                case "stcp":
                    dbHost = ConfigurationManager.AppSettings["RSTDBProd"];
                    break;

                default:
                    dbHost = ConfigurationManager.AppSettings["RSTDBDev"];
                    break;
            }

            return dbHost;
        }

        public static string GetFtpHost()
        {
            var ftpHost = "";
            var asEnv = Environment.GetEnvironmentVariable("AS_ENV") ?? "";
            switch (asEnv.ToLower())
            {
                case "alpha":
                    ftpHost = ConfigurationManager.AppSettings["EDWFTPAlpha"];
                    break;
                case "ppe1":
                    ftpHost = ConfigurationManager.AppSettings["EDWFTPBeta"];
                    break;
                case "hdcp":
                case "ntcp":
                case "dtcp":
                case "stcp":
                    ftpHost = ConfigurationManager.AppSettings["EDWFTPProd"];
                    break;
               
                default:
                    ftpHost = ConfigurationManager.AppSettings["EDWFTPAlpha"];
                    break;
            }

            return ftpHost;
        }

        public static string GetFtpPath()
        {
            var ftpPath = "";
            var asEnv = Environment.GetEnvironmentVariable("AS_ENV") ?? "";
            switch (asEnv.ToLower())
            {
                case "alpha":
                    ftpPath = ConfigurationManager.AppSettings["FTPPathAlpha"];
                    break;
                case "ppe1":
                    ftpPath = ConfigurationManager.AppSettings["FTPPathBeta"];
                    break;
                case "hdcp":
                case "ntcp":
                case "dtcp":
                case "stcp":
                    ftpPath = ConfigurationManager.AppSettings["FTPPathProd"];
                    break;

                default:
                    ftpPath = ConfigurationManager.AppSettings["FTPPathAlpha"];
                    break;
            }

            return ftpPath;
        }

        public static string GetAppDBConnStr(string db)
        {
            var dbHost = "";
            var asEnv = GetEnv();
            var dbprefix = "App";

            if (db.ToLower() == "ops") dbprefix = "Ops";

            if (db.ToLower() == "apv") dbprefix = "Apv";

            if (db.ToLower() == "rst") dbprefix = "RST";

            switch (asEnv.ToLower())
            {
                case "dev":
                    dbHost = ConfigurationManager.AppSettings[dbprefix+"DBDev"];
                    break;
                case "alpha":
                    dbHost = ConfigurationManager.AppSettings[dbprefix+"DBAlpha"];
                    break;
                case "ppe1":
                    dbHost = ConfigurationManager.AppSettings[dbprefix+"DBBeta"];
                    break;
                case "hdcp":
                case "ntcp":
                case "dtcp":
                case "stcp":
                    dbHost = ConfigurationManager.AppSettings[dbprefix+"DBProd"];
                    break;

                default:
                    dbHost = ConfigurationManager.AppSettings[dbprefix+"DBDev"];
                    break;
            }

            return dbHost;
        }

        public static int ThreadLimit
        {
            get
            {
                var setting = GetDBConfig("ThreadLimit");
                if (!string.IsNullOrEmpty(setting))
                {
                    _threadLimit = Convert.ToInt32(setting);
                }
                else
                {
                    ALogger.LogInfo("Can't get _threadLimit setting from DB use default value {0}", _threadLimit);
                }
                return _threadLimit;
            }
            set
            {
                _threadLimit = value;
                SetDBConfig("ThreadLimit", value.ToString(), "Dump to OpsConsole Thread Limit");
            }
        }

        public static string GetDBConfig(string configName)
        {
            var configValue = "";

            if (string.IsNullOrEmpty(configName))
            {
                ALogger.LogWarn("ConfigUitl.GetDBConfig(): config name can't be null or empty");
                return null;
            }

            using (var conn = OpsConsoleDB.CreateSqlCon())
            {
                if (conn == null)
                {
                    ALogger.LogError("ConfigUitl.GetDBConfig(): Could not open a connection to the Database");
                    return null;
                }

                var command = conn.CreateCommand();
                command.CommandTimeout = ConfigUtil.DBQueryTimeout;
                command.CommandText = string.Format(
                    "SELECT ConfigName, " +
                        "ConfigValue " +
                    "FROM U_Config " +
                    "WHERE ConfigName = @configName"
                    );
                configName = _DBConfigSettingPrefix + configName;
                command.Parameters.AddWithValue("@configName", configName);

                using (var dataReader = command.ExecuteReader())
                {
                    if (!dataReader.HasRows)
                    {
                        ALogger.LogWarn("ConfigUitl.GetDBConfig(): no any record data in specific setting {0}", configName);
                        return "";
                    }

                    while (dataReader.Read())
                    {
                        var record = dataReader as IDataRecord;
                        configValue = (string)record["ConfigValue"];
                    }
                }
            }

            return configValue;
        }

        public static bool SetDBConfig(string configName, string configValue, string description)
        {
            if (string.IsNullOrEmpty(configName))
            {
                ALogger.LogWarn("ConfigUitl.GetDBConfig(): config name can't be null or empty");
                return false;
            }

            using (var conn = OpsConsoleDB.CreateSqlCon())
            {
                if (conn == null)
                {
                    ALogger.LogError("ConfigUtill.GetDBConfig(): Could not open a connection to the Database");
                    return false;
                }

                SqlCommand cmdCount = new SqlCommand("SELECT count(*) from U_Config WHERE ConfigName = @configName", conn);
                cmdCount.CommandTimeout = ConfigUtil.DBQueryTimeout;
                configName = _DBConfigSettingPrefix + configName;
                cmdCount.Parameters.AddWithValue("@configName", configName);
                int count = (int)cmdCount.ExecuteScalar();

                SqlCommand command;
                if (count > 0)
                {
                    command = new SqlCommand("UPDATE U_Config SET ConfigName = @name, ConfigValue = @value, ConfigDescription = @desc WHERE ConfigName = @name", conn);
                }
                else
                {
                    command = new SqlCommand("INSERT into U_Config (ConfigName, ConfigValue, ConfigDescription) VALUES (@name, @value, @desc)", conn);
                }

                command.Parameters.AddWithValue("@name", configName);
                command.Parameters.AddWithValue("@value", configValue);
                command.Parameters.AddWithValue("@desc", description);
                int rowsUpdated = command.ExecuteNonQuery();

                ALogger.LogInfo("ConfigUtill.SetDBConfig(): setting {0} has value {1} with description {2} has {3} row(s) updated", configName, configValue, description, rowsUpdated);
                return rowsUpdated > 0? true: false;
            }
        }
    }
}
