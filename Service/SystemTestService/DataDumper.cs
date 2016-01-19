using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Compression;
using TR.AppServer.Common.Interfaces;
using TR.AppServer.Logging;
using WinSCP;
using SystemTestService.Utility;
using System.Diagnostics;
using System.Globalization;

namespace SystemTestService
{
    public class DataDumper
    {
        private static readonly ILogger ALogger = Logger.Default;
        private const string timeFormat = "yyyy-MM-dd HH:mm:ss";
        private int? _recordLimit = null;
        public int? RecordLimit
        {
            get { return _recordLimit; }
            set { _recordLimit = value; }
        }

        public List<TestResult> TestResult = null;

        //mode: [1:DumpOnly],[2:Dump+FTP],[3:Dump+Upload],[4:Dump+FTP+Upload]
        public string DumpRSTStatInterval(string _localSTSaveFolder, string start = "", string end = "", int mode = 4)
        {
            var msg = "";
            var dumpData = false;
            var fixPeriod = !String.IsNullOrEmpty(start) && !String.IsNullOrEmpty(end);
            var timer = new Stopwatch();
            try
            {
                var startTime = GetDumpTime(start);
                timer.Start();
                do
                {
                    ALogger.LogInfo("DumpRSTStatInterval: [STAGE-1-DUMP] : query ST stat for {0}: mode {1}", Formator.FormatDateTime(startTime),mode);
                    var plainTextDumpZip = DumpRSTStat(_localSTSaveFolder, startTime);
                    switch (plainTextDumpZip)
                    {
                        case null:
                            UpdateLastRun(startTime, fixPeriod);
                            ALogger.LogWarn("DumpRSTStatInterval: [STAGE-1-DUMP] :cannot query Ops stat/no any stat, {0}", Formator.FormatDateTime(startTime));
                            msg = "DumpRSTStatInterval: cannot query Ops stat/no any stat, " + Formator.FormatDateTime(startTime);
                            break;
                        case "exception":
                            ALogger.LogWarn("DumpRSTStatInterval: [STAGE-1-DUMP] : exception");
                            msg = "DumpRSTStatInterval: failed to get stat please see log for more details";
                            break;
                        default:
                            if((mode == 2 || mode == 4))
                            {
                                if (FtpUploadAll(_localSTSaveFolder))
                                {
                                    UpdateLastRun(startTime, fixPeriod);
                                    ALogger.LogInfo("DumpRSTStatInterval: [STAGE-2-FTP] : successfully upload {0}", plainTextDumpZip);
                                    msg = "DumpRSTStatInterval: successfully upload " + plainTextDumpZip;
                                }
                                else
                                {
                                    ALogger.LogInfo("DumpRSTStatInterval: [STAGE-2-FTP] : failed to upload {0}", plainTextDumpZip);
                                    msg = "DumpRSTStatInterval: failed to upload " + plainTextDumpZip;
                                }
                            }

                            if ((mode == 3 || mode == 4))
                            {
                                ALogger.LogInfo("DumpRSTStatInterval: [STAGE-3-OPS] : Sending data to OpsConsole ...");
                                SendDataToOpsConsole();
                                dumpData = true;
                            }                                                            
                            break;
                    }
                    startTime = fixPeriod?startTime.AddMinutes(ConfigUtil.JobDumpIntervalMinute):GetDumpTime();
                } while (startTime < GetDumpTime(end, !fixPeriod)); //if not fix periord, dump until now.

                if (dumpData)
                {
                    ALogger.LogInfo("DumpRSTStatInterval: Merging stat to OpsConsole ...");
                    MergeOpsConsoleTestID();
                }

            }
            catch (Exception ex)
            {
                ALogger.LogError("DumpRSTStatInterval: Error {0}", ex.Message);
                msg = "DumpRSTStatInterval: Error " + ex.Message;
            }

            timer.Stop();
            ALogger.LogInfo("DumpRSTStatInterval: [STAGE-4-DONE] : Query Stat from {0} to {1} in {2} minutes", start, end, timer.Elapsed.Minutes);
            return msg;
        }

        public static void MergeOpsConsoleTestID()
        {
            try
            {
                using (var conn = OpsConsoleDB.CreateSqlCon())
                {
                    if (conn == null)
                    {
                        ALogger.LogError("Could not open a connection to the Database");
                        return;
                    }
                    var timer = new Stopwatch();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandTimeout = 600;
                        cmd.CommandText =
                        "MERGE [UserMachineInstallationTime] as target " +
                            "USING (SELECT Distinct UserMachineInstallationID, UTCTime  " +
                                "FROM UserStatsDetails " +
                                "WHERE Product = 'est') AS source " +
                        "ON (target.UserMachineInstallationID = source.UserMachineInstallationID AND target.UTCTime = source.UTCTime) " +
                        "WHEN NOT MATCHED THEN INSERT (UserMachineInstallationID, UTCTime) " +
                            "VALUES (source.UserMachineInstallationID, source.UTCTime);";
                        timer.Start();
                        var rowAffected = cmd.ExecuteNonQuery();
                        timer.Stop();
                        ALogger.LogInfo("MergeOpsConsoleTestID: UserMachineInstallationTime {0} row(s) affected, {1} milliseconds", rowAffected, timer.ElapsedMilliseconds);

                        cmd.CommandText =
                            "SELECT d1.UserMachineInstallationID, d1.UTCTime, d1.testID " +
                            " INTO  #T " +
                            " FROM " +
                                /* list only test that have  test ID */
                                "(SELECT MIN(d.UserMachineInstallationID) AS UserMachineInstallationID, MIN(d.UTCTime) AS UTCTime " +
                                ",MIN(t.UserMachineInstallationTimeID) AS testID " +
                                "FROM  UserStatsDetails d LEFT JOIN UserMachineInstallationTime t " +
                                "ON (d.UserMachineInstallationID = t.UserMachineInstallationID AND d.UTCTime = t.UTCTime) " +
                                "WHERE d.Product = 'est' AND " +
                                    "t.UserMachineInstallationTimeID is not null " +
                                "GROUP BY d.UserMachineInstallationID, d.UTCTime) d1 " +
                            "LEFT JOIN " +
                                /* list only test that have 'testID' case */
                                "(SELECT MIN(d.UserMachineInstallationID) AS UserMachineInstallationID, MIN(d.UTCTime) AS UTCTime " +
                                "FROM UserStatsDetails d " +
                                "WHERE d.Product = 'est' AND d.StatCode = 'testID' " +
                                "GROUP BY d.UserMachineInstallationID, d.UTCTime) t1 " +
                            "ON (d1.UserMachineInstallationID = t1.UserMachineInstallationID AND d1.UTCTime = t1.UTCTime) " +
                            "WHERE t1.UserMachineInstallationID is null";
                        timer.Start();
                        rowAffected = cmd.ExecuteNonQuery();
                        timer.Stop();
                        ALogger.LogInfo("MergeOpsConsoleTestID: temp table #T {0} row(s) affected, {1} milliseconds", rowAffected, timer.ElapsedMilliseconds);

                        cmd.CommandText =
                            "INSERT INTO UserStatsDetails " +
                            "(UserMachineInstallationID, UTCTime, StatCode, StatValue, Product)" +
                            "SELECT UserMachineInstallationID, UTCTime, 'testID', testID, 'est' FROM #T";
                        timer.Start();
                        rowAffected = cmd.ExecuteNonQuery();
                        timer.Stop();
                        ALogger.LogInfo("MergeOpsConsoleTestID: UserStatsDetails {0} row(s) affected, {1} milliseconds", rowAffected, timer.ElapsedMilliseconds);
                    }
                }
            }
            catch (Exception e)
            {
                ALogger.LogError("MergeOpsConsoleTestID: exception, ", e.Message);
            }

        }
        public static string CleanupFile(string path, bool list = false, int keepDay = 7)
        {
            var listResult = "";
            try
            {
                var cleanPath = path;
                DirectoryInfo localSaveDirectory;
                Action<FileInfo> deleter = (f =>
                {
                    if (list)
                    {
                        listResult += "," + f.FullName;
                    }
                    else
                    {
                        ALogger.LogInfo("\t\tdeleting {0}", f.FullName);
                        try
                        {
                            File.Delete(f.FullName);
                        }
                        catch (Exception ex)
                        {
                            ALogger.LogError("CleanupFile: {0} exception {1}", f.FullName, ex.Message);
                        }
                    }
                });

                if (Directory.Exists(cleanPath))
                {
                    localSaveDirectory = new DirectoryInfo(cleanPath);
                    localSaveDirectory.GetFiles().Where(x => x.CreationTime.Date <= DateTime.Today.AddDays(-keepDay)).ToList().ForEach(deleter);   
                }
                
                var archivePath = Path.Combine(cleanPath, "Archive");
                if (Directory.Exists(archivePath))
                {
                    localSaveDirectory = new DirectoryInfo(Path.Combine(cleanPath, "Archive"));
                    localSaveDirectory.GetFiles().Where(x => x.CreationTime.Date <= DateTime.Today.AddDays(-keepDay)).ToList().ForEach(deleter);
                }
            }
            catch (Exception ex)
            {
                ALogger.LogError("CleanupFile: exception {0}", ex.Message);
                return listResult + ":- Error: " + ex.Message;
            }
            return listResult;
        }
        public static string CleanUpTempFile(string targetFolder, bool list)
        {
            try
            {
                var localSaveDirectory = new DirectoryInfo(targetFolder);

                Action<FileInfo> deleter = (f =>
                {
                    ALogger.LogInfo("\t\tdeleting {0}", f.FullName);
                    File.Delete(f.FullName);
                });

                if (list)
                {
                    var deleteList = localSaveDirectory.GetFiles().Where(x => x.FullName.EndsWith(".json")).ToList().Select(file => file.Name).ToList();
                    return string.Join(",", deleteList.ToArray());
                }
                
                localSaveDirectory.GetFiles().Where(x => x.FullName.EndsWith(".json")).ToList().ForEach(deleter);
                return "Done";
            }
            catch (Exception ex)
            {
                ALogger.LogError("CleanUpTempFile: exception {0}", ex.Message);
                return "Error";
            }
        }
        
        private string DumpRSTStat(string localSaveFolder, DateTime dateToDump)
        {            
            var fileName = Formator.FormatFileName(dateToDump);

            if (!Directory.Exists(localSaveFolder))
            {
                Directory.CreateDirectory(localSaveFolder);
            }

            // Do not dispose objects multiple times => msdn.microsoft.com/en-us/library/ms182334.aspx
            FileStream fs = null;
            StreamWriter stream = null;
            JsonTextWriter jsonSerializer = null;
            var statJsonFile = Path.Combine(localSaveFolder, fileName + ".json");
            try
            {
                fs = new FileStream(statJsonFile, FileMode.Create);
                stream = new StreamWriter(fs);
                jsonSerializer = new JsonTextWriter(stream) {DateFormatString = "yyyy-MM-dd"};
                if (!ConfigUtil.UseOpsConsoleDBFlag)
                {
                    using (var conn = DBConnection.CreateSqlCon())
                    {
                        if (conn == null)
                        {
                            ALogger.LogError("Could not open a connection to the Database");
                            return null;
                        }

                        TestResult = GetTestResultFromSTDB(conn.CreateCommand(), jsonSerializer, dateToDump);
                        if (ConfigUtil.DumpNumberOfRecord)
                        {
                            if (TestResult.Count != 0)
                            {
                                WriteNumberOfRecord(TestResult.Count, localSaveFolder + "/NumberOfRecord.json", fileName);
                            }
                        }
                        fs.Flush(true);
                    }
                }
                else
                {
                    using (var conn = OpsConsoleDB.CreateSqlCon())
                    {
                        if (conn == null)
                        {
                            ALogger.LogError("Could not open a connection to the Database");
                            return null;
                        }

                        TestResult = GetTestResultFromOpsDB(conn.CreateCommand(), jsonSerializer, dateToDump);
                        if (ConfigUtil.DumpNumberOfRecord)
                        {
                            if (TestResult.Count != 0)
                            {
                                WriteNumberOfRecord(TestResult.Count, localSaveFolder + "/NumberOfRecord.json", fileName);
                            }
                        }
                        fs.Flush(true);
                    }
                }
                
            }
            catch (Exception ex)
            {
                ALogger.LogError("Error getting/writing data dump: {0}", ex.Message);
                File.Delete(statJsonFile);
                return null;
            }
            finally
            {
                if (jsonSerializer != null)
                    jsonSerializer.Close();
                if (stream != null)
                    stream.Dispose();
                if (fs != null)
                    fs.Dispose();
            }
            return ProcessDumpFile(statJsonFile, localSaveFolder, fileName);
        }
        private List<TestResult> GetTestResultFromSTDB(SqlCommand command, JsonWriter serializer, DateTime dateToDump)
        {
            var startTime = Formator.FormatDateTime(dateToDump.AddMinutes(-ConfigUtil.JobDumpIntervalMinute));
            var endTime = Formator.FormatDateTime(dateToDump);

            ALogger.LogInfo("GetTestResultFromSTDB: Start {0} to {1} - Input {2}", startTime, endTime, dateToDump.ToString("yyyy-MM-dd HH:mm:ss"));

            if (command == null || serializer == null)
            {
                ALogger.LogError("Not allowed to pass null to this method");
                throw new NoNullAllowedException("Not allowed to pass null to this method");
            }

            var limit = "";

            if (_recordLimit != null && _recordLimit > 0)
            {
                limit = " TOP " + _recordLimit + " ";
            }

            command.CommandTimeout = ConfigUtil.DBQueryTimeout;
            command.CommandText = string.Format(
                //"WAITFOR DELAY '000:01:10';" +
                "SELECT "+ limit +" US.userID AS 'uuid', " +
                    "SE.serverName AS 'capb.name'," +
                    "SE.serverVersion AS 'capb.version', " +
                    "SE.serverCompName AS 'capb.server', " +
                    "(select case when PD.productName is null then '' else PD.productName end) AS 'prod.name', " +
                    "(select case when PD.productVersion is null then '' else PD.productVersion end) AS 'prod.vers', " +
                    "'RST' AS 'prod.component.name', " +
                    "((select case when CL.clientName is null then '' else CL.clientName end) + ' ' + (select case when " +
                    "CL.clientVersion is null then '' else CL.clientVersion end)) AS 'prod.component.vers',  " +
                    "CONVERT(varchar(10),TR.testID) AS 'testID', " +
                    "US.userID AS 'userProfile.uuid', " +
                    "(select case when US.userName is null then '' else US.userName end) AS 'userProfile.userName', " +
                    "(select case when US.userName is null then '' else US.userName end) AS 'userProfile.email', " +
                    "(select case when GL.geoCountry is null then '' else GL.geoCountry end) AS 'userProfile.geoCountry', " +
                    "SE.serverName AS 'server.name', " +
                    "SE.serverVersion AS 'server.version', " +
                    "SE.serverCompName AS 'server.compName', " +
                    "DC.dcSite AS 'server.dataCenter', " +
                    "'RST' AS 'client.name', " +
                    "((select case when CL.clientName is null then '' else CL.clientName end) + ' ' + (select case when CL.clientVersion is null then '' else CL.clientVersion end)) AS 'client.version',  " +
                    "(select case when CL.machineID is null then '' else CL.machineID end) AS 'machineID', " +
                    "(select case when RU.runMOdeKey is null then '' else RU.runMOdeDesc end) AS 'runningMode'," +
                    "(select case when TR.localDateTime is null then '' else TR.localDateTime end) AS 'localDateTime', " +
                    "(select case when TR.wasDateTime is null then '' else TR.wasDateTime end) AS 'server.wasDateTime', " +
                    "(select case when TR.dbsDateTime is null then '' else TR.dbsDateTime end) AS 'server.dbsDateTime', " +
                    "(select case when RU.runModeKey is null then '' else RU.runModeDesc end) AS 'runningMode', " +

                    "TC.testCaseID AS 'testcase.id', " +
                    "isnull(TG.testGroupID,'') AS 'testcase.group', " +
                    "isnull(TC.testCaseName,'') AS 'testcase.title', " +
                    "isnull(VL.validDesc,'') AS 'testcase.valid', " +
                    "isnull(TCS.tcValue,'') AS 'testcase.value', " +
                    "(select case when isnull(TCS.tcDesc,'') = '-' then TCS.tcValue else isnull(TCS.tcDesc,'') end) AS 'testcase.description', " +
                    "isnull(TC.rphURL,'') AS 'testcase.rphURL', " +
                    "isnull(TCS.resultID,'') AS 'testcase.resultID', " +
                    "isnull(TCS.tcRecom,'') AS 'testcase.recommendation' " +

                "FROM	master_dataCenter DC, " +
                    "rstTestResult TR, " +
                    "master_user US, " +
                    "master_geoLocation GL, " +
                    "master_product PD, " +
                    "master_client CL, " +
                    "master_runMode RU, " +
                    "master_server SE, " +
                    "rstTestCases TCS, " +
                    "master_testGroup TG, " +
                    "master_testCase TC, " +
                    "master_valid VL " +
                "WHERE TR.dcKey = DC.dcKey " +
                    "AND TR.userKey = US.userKey " +
                    "AND TR.geoLocationKey = GL.geoLocationKey " +
                    "AND TR.productKey = PD.productKey " +
                    "AND TR.clientKey = CL.clientKey " +
                    "AND TR.runMOdeKey = RU.runMOdeKey " +
                    "AND TR.serverKey = SE.serverKey " +
                    "AND TR.testKey          = TCS.testKey " +
                    "AND TCS.validKey     = VL.validKey " +
                    "AND TCS.testGroupKey = TG.testGroupKey " +
                    "AND TCS.testCaseKey     = TC.testCaseKey " +
                    "AND TR.dbsDateTime >= convert(datetime, '" + startTime + "') " +
                    "AND TR.dbsDateTime <= convert(datetime, '" + endTime + "') " +
                    "ORDER BY TR.testID,testCaseOrd,testCaseName "
                );

            int numberOfTestID = 0;
            List<TestResult> testResult = null;
            using (var dataReader = command.ExecuteReader())
            {
                var previousTestReferenceID = string.Empty;
                
                while (dataReader.Read())
                {
                    var record = dataReader as IDataRecord;
                    var testID = (string)record["testID"];

                    if (testID != previousTestReferenceID)
                    {
                        if (testResult == null)
                        {
                            testResult = new List<TestResult>();
                        }

                        testResult.Add(new TestResult
                        {
                            UUID = (string)record["uuid"],
                            CapbName = (string)record["capb.name"],
                            CapbVers = (string)record["capb.version"],
                            CapbServer = (string)record["capb.server"],
                            ProdName = (string)record["prod.name"],
                            ProdVers = (string)record["prod.vers"],
                            ProdComponentName = (string)record["prod.component.name"],
                            ProdComponentVers = (string)record["prod.component.vers"],
                            TestID = Convert.ToInt32(record["testID"]),
                            UserProfileUUID = (string)record["userProfile.uuid"],
                            //UserProfileAccountDomain = (string)record["userProfile.accountDomain"],
                            UserProfileUserName = (string)record["userProfile.userName"],
                            UserProfileEmail = (string)record["userProfile.email"],
                            //UserProfileContactName = (string)record["userProfile.contactName"],
                            //UserProfileLocation = (string)record["userProfile.location"],
                            //UserProfileCID = (string)record["userProfile.CID"],
                            UserProfileGeoCountry = (string)record["userProfile.geoCountry"],
                            ServerName = (string)record["server.name"],
                            ServerVersion = (string)record["server.version"],
                            ServerCompName = (string)record["server.compName"],
                            ServerDatacenter = (string)record["server.dataCenter"],
                            ServerWasDateTime = (DateTime)record["server.wasDateTime"],
                            ServerDbsDateTime = (DateTime)record["server.dbsDateTime"],
                            ClientName = (string)record["client.name"],
                            ClientVersion = (string)record["client.version"],
                            MachineID = (string)record["machineID"],
                            RunningMode = (string)record["runningMode"],
                            LocalDateTime = (DateTime)record["localDateTime"],
                            TestCases = new List<TestCase>()
                        });

                        previousTestReferenceID = testID;
                        numberOfTestID++;
                    }

                    testResult[numberOfTestID - 1].TestCases.Add(
                        new TestCase
                        {
                            ID = (string) record["testcase.id"],
                            Title = (string) record["testcase.title"],
                            Group = (string) record["testcase.group"],
                            Valid = (string) record["testcase.valid"],
                            Value = (string) record["testcase.value"],
                            Description = (string) record["testcase.description"],
                            Recommendation = (string) record["testcase.recommendation"],
                            RphURL = (string) record["testcase.rphURL"],
                            ResultID = (string) record["testcase.resultID"]
                        }
                    );
                }

                if (numberOfTestID != 0)
                {
                    for (var i = 0; i < numberOfTestID; i++)
                    {
                        var json = JsonConvert.SerializeObject(testResult[i]); 
                        serializer.WriteStartObject();
                        serializer.WriteRaw(json);
                        serializer.WriteEndObject();
                        serializer.WriteWhitespace(Environment.NewLine);
                    }
                }
            }

            return testResult;
        }
        private List<TestResult> GetTestResultFromOpsDB(SqlCommand command, JsonWriter serializer, DateTime dateToDump)
        {
            var compName = Environment.GetEnvironmentVariable("COMPUTERNAME") ?? "";
            var envName = Environment.GetEnvironmentVariable("AS_ENV") ?? "";
            var startTime = Formator.FormatDateTime(dateToDump.AddMinutes(-ConfigUtil.JobDumpIntervalMinute));
            var endTime = Formator.FormatDateTime(dateToDump);

            ALogger.LogInfo("GetTestResultFromOpsDB: Start {0} to {1} - Input {2}", startTime, endTime, dateToDump.ToString("yyyy-MM-dd HH:mm:ss"));

            if (command == null || serializer == null)
            {
                ALogger.LogError("Not allowed to pass null to this method");
                throw new NoNullAllowedException("Not allowed to pass null to this method");
            }

            var limit = "";

            if (_recordLimit != null && _recordLimit > 0)
            {
                limit = " TOP " + _recordLimit + " ";
            }

            command.CommandTimeout = ConfigUtil.DBQueryTimeout;
            command.CommandText = string.Format(
                //"WAITFOR DELAY '000:01:10';" +
                "SELECT " + limit + " m.UUID as 'uuid', " +
                    "'SystemTestApp' AS 'capb.name', " +
                    "'1.0' AS 'capb.version', " +
                    "'" + compName + "' AS 'capb.server', " +                    
                    //"*** '62' **** AS 'prod.name',  " +
                    //"*** '127EIKON' ***** AS 'prod.vers', " +
                    "'RST' AS 'prod.component.name', " +
                    //"*** '127ST' **** AS 'prod.component.vers',  " +
                    //"'' AS 'testID', => null" +
                    "m.UUID AS 'userProfile.uuid', " +
                    /* UserProfileAccountDomain => null*/
                    // "*** 61 **** AS 'userProfile.userName',  " +
                    //"*** UIS **** AS 'userProfile.email', " +
                    //UserProfileContactName = (string)record["userProfile.contactName"],
                    //UserProfileLocation = (string)record["userProfile.location"],
                    //UserProfileCID = (string)record["userProfile.CID"],
                    //"*** UIS **** AS 'userProfile.geoCountry', " +
                    "'SystemTestApp' AS 'server.name', " +
                    "'1.0' AS 'server.version', " +
                    "'" + compName + "' AS 'server.compName', " +
                    "'" + envName + "' AS 'server.dataCenter', " +
                    "s.LastSeen AS 'server.wasDateTime', " +
                    "s.LastSeen AS 'server.dbsDateTime', " +
                    "'RST' AS 'client.name', " +
                    //"*** '127ST' ****  AS 'client.version', " +
                    "m.MachineGUID AS 'machineID', " +
                    "'' AS 'runningMode', " +
                    //"*** 73 **** AS 'localDateTime', " +
                   
                    "s.StatCode AS 'testcase.id', " +
                    "'' AS 'testcase.group', " +
                    "'' AS 'testcase.title', " +
                    "'' AS 'testcase.valid', " +
                    "s.StatValue AS 'testcase.value', " +
                    "'' AS 'testcase.description', " +
                    "'' AS 'testcase.rphURL', " +
                    "'' AS 'testcase.resultID', " +
                    "'' AS 'testcase.recommendation' " +

                "FROM UserStats s LEFT JOIN UserMachineInstallations m " +
                "ON (s.UserMachineInstallationID = m.UserMachineInstallationID) " +
                "WHERE m.Product = 'est' " +
                    "AND s.LastSeen >= convert(datetime, '" + startTime + "') " +
                    "AND s.LastSeen <= convert(datetime, '" + endTime + "') "
                );

            var numberOfTestID = 0;
            List<TestResult> testResult = null;
            using (var dataReader = command.ExecuteReader())
            {
                var previousTestReferenceID = new DateTime(0);

                while (dataReader.Read())
                {
                    var record = dataReader as IDataRecord;
                    var timeStamp = (DateTime)record["server.dbsDateTime"];
                    if (timeStamp != previousTestReferenceID)
                    {                        
                        if (testResult == null)
                        {
                            testResult = new List<TestResult>();
                        }
                        var userDetails = UserInfo.GetUserDetails((string) record["uuid"]);
                        testResult.Add(new TestResult
                        {
                            UUID = (string)record["uuid"],
                            CapbName = (string)record["capb.name"],
                            CapbVers = (string)record["capb.version"],
                            CapbServer = (string)record["capb.server"],
                            //ProdName = (string)record["prod.name"], //  62
                            //ProdVers = (string)record["prod.vers"], // 127EIKON
                            ProdComponentName = (string)record["prod.component.name"],
                            //ProdComponentVers = (string)record["prod.component.vers"], // 127ST
                            //TestID = Convert.ToInt32(record["testID"]), null
                            UserProfileUUID = (string)record["userProfile.uuid"],
                            //UserProfileAccountDomain = (string)record["userProfile.accountDomain"],  null
                            UserProfileUserName = (userDetails != null) ? userDetails.UserID : "",// UIS
                            UserProfileEmail = (userDetails != null) ? userDetails.Email : "", // UIS
                            //UserProfileContactName = (string)record["userProfile.contactName"], => null
                            //UserProfileLocation = (string)record["userProfile.location"], => null
                            //UserProfileCID = (string)record["userProfile.CID"], => null
                            UserProfileGeoCountry = (userDetails != null) ? userDetails.Country : "", // UIS
                            ServerName = (string)record["server.name"],
                            ServerVersion = (string)record["server.version"],
                            ServerCompName = (string)record["server.compName"],
                            ServerDatacenter = (string)record["server.dataCenter"],
                            ServerWasDateTime = (DateTime)record["server.wasDateTime"],
                            ServerDbsDateTime = (DateTime)record["server.dbsDateTime"],
                            ClientName = (string)record["client.name"],
                            //ClientVersion = (string)record["client.version"], // 127ST
                            MachineID = record["machineID"].ToString(),
                            RunningMode = (string)record["runningMode"], // blank
                            //LocalDateTime = (DateTime)record["localDateTime"], // 73
                            TestCases = new List<TestCase>()
                        });

                        previousTestReferenceID = timeStamp;
                        numberOfTestID++;
                    }

                    var statCode = DataMapper.MapStatCode((string)record["testcase.id"]);
                    var statValue = (string)record["testcase.value"];
                    testResult[numberOfTestID - 1].TestCases.Add(
                        new TestCase
                        {
                            ID = statCode,
                            Title = (string)record["testcase.title"], // blank
                            Group = (string)record["testcase.group"], // blank
                            Valid = (string)record["testcase.valid"], // blank
                            Value = statValue,
                            Description = (string)record["testcase.description"], // blank
                            Recommendation = (string)record["testcase.recommendation"], // blank
                            RphURL = (string)record["testcase.rphURL"], // blank
                            ResultID = (string)record["testcase.resultID"] // blank
                        }
                    );

                    switch (statCode)
                    {
                        case "62":
                            testResult[numberOfTestID - 1].ProdName = statValue;
                            break;
                        case "127EIKON":
                            testResult[numberOfTestID - 1].ProdVers = statValue;
                            break;
                        case "127ST":
                            testResult[numberOfTestID - 1].ProdComponentVers = statValue;
                            testResult[numberOfTestID - 1].ClientVersion = statValue;                            
                            break;
                        case "27": // timezone                            
                            testResult[numberOfTestID - 1].LocalDateTime = GetLocalTestTime(testResult[numberOfTestID - 1].ServerWasDateTime, statValue);
                            break;
                        /*case "73": // local time
                            
                            break;*/
                    }
                }

                if (numberOfTestID == 0)
                {
                    ALogger.LogWarn("DataDumper.GetTestResultFromOpsDB(): no any test data in specific peroid [{0} until {1}]", startTime, endTime);
                    return testResult;
                }
                    
                for (var i = 0; i < numberOfTestID; i++)
                {
                    var json = JsonConvert.SerializeObject(testResult[i]);
                    serializer.WriteStartObject();
                    serializer.WriteRaw(json);
                    serializer.WriteEndObject();
                    serializer.WriteWhitespace(Environment.NewLine);
                }
            }

            return testResult;
        }                
        private void SendDataToOpsConsole()
        {
            if (!ConfigUtil.UseOpsConsoleDBFlag)
            {
                var logStatList = UpdateOpsConsoleDb.ConvertToLogStat(TestResult);
                if (logStatList != null)
                {
                    UpdateOpsConsoleDb.SendDataToOpsConsole(logStatList);
                }
                else
                {
                    ALogger.LogWarn("no any stat !!!");
                }
            }
            TestResult = null;
        }
        private static DateTime GetLocalTestTime(DateTime gmtDateTime, string timeZone)
        {
            var regex = new Regex(@"\d{1,2}:\d{1,2}\) (.*)$");
            try
            {
                var matches = regex.Matches(timeZone);
                var zoneID = matches[0].Groups[1].Value.Trim();
                if (string.IsNullOrEmpty(zoneID)) return gmtDateTime;
                var clientZone = TimeZoneInfo.FindSystemTimeZoneById(zoneID);
                return TimeZoneInfo.ConvertTime(gmtDateTime, TimeZoneInfo.Local, clientZone);
            }
            catch (Exception)
            {
                ALogger.LogWarn("DataDumper.GetLocalTestTime(): cannot get timezone for {0}", timeZone);
            }
            return gmtDateTime;
        }
        private DateTime GetDumpTime(string date = "", bool nowflag = false)
        {
            var nowDate = DateTime.Now;
            var nextRunNow = nowDate.Date + new TimeSpan(nowDate.Hour, nowDate.Minute, 0);
            //Add dump delay config to the current time.
            nextRunNow = nextRunNow.AddMinutes(-ConfigUtil.JobDumpDelayMinute);
            var statDateTobeQuery = nextRunNow;


            if (nowflag)
            {
                return nextRunNow;
            }

            try
            {
                if (!string.IsNullOrEmpty(date))
                {
                    //If specific datetime there is no dump delay included.
                    statDateTobeQuery = DateTime.ParseExact(date, timeFormat, System.Globalization.CultureInfo.InvariantCulture);
                    return statDateTobeQuery;
                }
                else
                {
                    var lastRunSetting = ConfigUtil.GetDBConfig("LastSTRun");
                    DateTime nextRunLast;
                    if (string.IsNullOrEmpty(lastRunSetting))
                    {
                        nextRunLast = nextRunNow;
                    }
                    else
                    {
                        const DateTimeStyles style = new System.Globalization.DateTimeStyles();
                        if (DateTime.TryParseExact(lastRunSetting, timeFormat, System.Globalization.CultureInfo.InvariantCulture, style, out nextRunLast))
                        {
                            nextRunLast = nextRunLast.AddMinutes(ConfigUtil.JobDumpIntervalMinute);
                            nextRunLast = nextRunLast.Date + new TimeSpan(nextRunLast.Hour, nextRunLast.Minute, 0);
                            ALogger.LogInfo("SystemTestService.GetDumpTime: Last Run setting {0}, Next Run Last {1}, Next Run Now {2}, Now {3}",
                                lastRunSetting, nextRunLast.ToString(timeFormat), nextRunNow.ToString(timeFormat), statDateTobeQuery.ToString(timeFormat));
                        }
                    }
                    //Compare and select the earliest time.
                    statDateTobeQuery = nextRunLast < nextRunNow ? nextRunLast : nextRunNow;
                }
            }
            catch (Exception e)
            {
                ALogger.LogError("SystemTestService.GetDumpTime: failed to get/convert datetime {0} {1}", date, e.Message);
                throw (e);
            }
            ALogger.LogInfo("SystemTestService.GetDumpTime: statDateTobeQuery {0}", statDateTobeQuery.ToString(timeFormat));
            return statDateTobeQuery;
        }
        private static void UpdateLastRun(DateTime startTime, bool fixPeriod)
        {
            if (!fixPeriod)
            {
                ConfigUtil.SetDBConfig("LastSTRun", startTime.ToString(timeFormat), "Last Success Run of ST Data Dump to BI");
            }
        }
        private string ProcessDumpFile(string statJsonFile, string localSaveFolder, string fileName)
        {
            if (new FileInfo(statJsonFile).Length == 0)
            {
                ALogger.LogWarn("0 length of stat json file");
                File.Delete(statJsonFile);
                return null;
            }

            var tmpFolder = Path.Combine(localSaveFolder, fileName);
            Directory.CreateDirectory(tmpFolder);
            foreach (var jsonFile in Directory.EnumerateFiles(localSaveFolder, "*.json"))
            {
                var jsonFileName = Path.GetFileName(jsonFile);
                var newPath = Path.Combine(tmpFolder, jsonFileName);
                File.Move(jsonFile, newPath);
            }

            var plainTextDumpZip = tmpFolder + ".gz";
            if (File.Exists(plainTextDumpZip))
            {
                File.Delete(plainTextDumpZip);
            }
            ZipFile.CreateFromDirectory(tmpFolder, plainTextDumpZip);
            Directory.Delete(tmpFolder, true);

            return plainTextDumpZip;
        }
        private bool FtpUploadAll(string localSaveFolder)
        {
            var resultFlag = true;

            if (!ConfigUtil.Ftp2Bi)
            {
                ALogger.LogInfo("FTP2BI Setting is false");
                return true;
            }

            try
            {
                var sessionOptions = new SessionOptions
                {
                    Protocol = Protocol.Sftp,
                    GiveUpSecurityAndAcceptAnySshHostKey = true,
                    HostName = ConfigUtil.GetFtpHost(),
                    UserName = ConfigUtil.EDWFTPUser,
                    Password = ConfigUtil.EDWFTPPassword,
                    PortNumber = ConfigUtil.EDWFTPPort
                };
                using (var session = new Session
                {
                    ExecutablePath = Path.Combine("winscp", "WinSCP.exe"),
                    Timeout = TimeSpan.FromSeconds(30),
                })
                {
                    try
                    {
                        session.Open(sessionOptions);
                        var remoteFilePath = "";
                        var archiveFolder = Path.Combine(localSaveFolder, "Archive");
                        foreach (var fileInfo in GetFTPFileList(localSaveFolder))
                        {
                            try
                            {
                                remoteFilePath = Path.Combine(ConfigUtil.GetFtpPath(), fileInfo.Name).Replace('\\', '/');
                                var result = session.PutFiles(fileInfo.FullName, remoteFilePath);
                                if (result.IsSuccess)
                                {
                                    ALogger.LogInfo("successfully upload file, {0} to {1}", fileInfo.FullName, remoteFilePath);
                                    var newPath = Path.Combine(archiveFolder, fileInfo.Name);
                                    if (!Directory.Exists(archiveFolder)) Directory.CreateDirectory(archiveFolder);
                                    try
                                    {
                                        File.Move(fileInfo.FullName, newPath);
                                    }
                                    catch (Exception)
                                    {
                                    }

                                }
                                else
                                {
                                    ALogger.LogError("failed to upload file, {0} to {1}", fileInfo.FullName, remoteFilePath);
                                    foreach (var failed in result.Failures)
                                    {
                                        ALogger.LogError("\t\t###{0}###", failed);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                ALogger.LogError("failed to upload file, {0} with error {1}", remoteFilePath, ex.Message);
                                resultFlag = false;
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        ALogger.LogError("FtpUpload: exception {0}", ex.Message);
                        resultFlag = false;
                    }

                }
            }
            catch (Exception e)
            {
                ALogger.LogError("FtpUpload(): exception, {0}", e.Message);
                resultFlag = false;
            }
            return resultFlag;
        }
        private List<FileInfo> GetFTPFileList(string localSaveFolder)
        {
            var localSaveDirectory = new DirectoryInfo(localSaveFolder);
            var filelist = localSaveDirectory.GetFiles("*.gz").OrderBy(y => y.CreationTime).ToList();
            return filelist;
        }
        private void WriteNumberOfRecord(int numberOfRecord, string jsonPath, string statPath)
        {
            FileStream fs = null;
            StreamWriter stream = null;
            JsonTextWriter jsonSerializer = null;
            try
            {
                fs = new FileStream(jsonPath, FileMode.Create);
                stream = new StreamWriter(fs);
                jsonSerializer = new JsonTextWriter(stream);

                jsonSerializer.Formatting = Formatting.Indented;
                jsonSerializer.WriteStartObject();
                jsonSerializer.WritePropertyName("StatJsonFile");
                jsonSerializer.WriteValue(statPath);
                jsonSerializer.WritePropertyName("NumberOfRecord");
                jsonSerializer.WriteValue(numberOfRecord);
                jsonSerializer.WriteEndObject();
                fs.Flush(true);
            }
            catch (Exception ex)
            {
                ALogger.LogError("Error writing number of record file: {0}", ex.Message);
            }
            finally 
            {
                if (jsonSerializer != null)
                    jsonSerializer.Close();
                if (stream != null)
                    stream.Dispose();
                if (fs != null)
                    fs.Dispose();
            }
        }
    }
}
