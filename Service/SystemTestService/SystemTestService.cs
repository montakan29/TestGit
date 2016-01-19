using System;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace SystemTestService
{
    public class SystemTestLoader : ISystemTestService
    {
        readonly string _localSTSaveFolder = Path.Combine(Environment.ExpandEnvironmentVariables("%AS_DATA%"), "RSTStat");

        public string DumpAndUploadRSTStatInterval(string start = "", string end = "")
        {
            var dumper = new DataDumper();
            var msg = dumper.DumpRSTStatInterval(_localSTSaveFolder, start, end);
            DataDumper.CleanupFile(_localSTSaveFolder);
            return msg;
        }

        public string GetData()
        {
            var sb = new StringBuilder("first 10 tables in rstdb are: ");
            using (var conn = DBConnection.CreateSqlCon())
            {              
                try
                {
                    var myCommand = new SqlCommand("select top 10 name from sys.tables", conn);
                    var myReader = myCommand.ExecuteReader();
                    while (myReader.Read())
                    {
                        sb.Append(myReader["name"] + ", ");
                    }
                    sb.Remove(sb.Length - 2, 1);
                    return sb.ToString();
                }
                catch (Exception ex)
                {
                    return "exception: " + ex.Message;
                }
                finally
                {
                    conn.Close();
                }
            }                       
        }
    }
}
