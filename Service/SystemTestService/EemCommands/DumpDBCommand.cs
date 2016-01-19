using System;
using System.Collections.Generic;
using System.ServiceModel.Dispatcher;
using System.Text;
using SystemTestService.Utility;
using EikonEnvManager.ProcessManagement;
using TR.AppServer.Common.Interfaces;
using TR.AppServer.Logging;
using System.Data;
using System.Data.SqlClient;

namespace SystemTestService.EemCommands
{
    class DumpDBCommand : CommandBase
    {
        private readonly ILogger _logger = Logger.Default;

        [CmdParam("Limit", DefaultValue = 10, Required = false, Description = "Enter limit number")]
        public int Limit { get; set; }

        [CmdParam("DB", DefaultValue = "App", Required = false, Description = "Enter DB (App,Ops,Apv): Apv-AppVersion")]
        public string DB { get; set; }

        [CmdParam("Query", DefaultValue = "", Required = true, Description = "Enter Query")]
        public string Query { get; set; }

        public DumpDBCommand()
            : base("Dump-App-DB", "Enter Query here to dump DB data (replace = with #)")
        {
        }

        public override CmdResult Execute()
        {
            if (string.IsNullOrWhiteSpace(Query))
            {
                return CmdResult.Failure("Query is null or empty");
            }

            try
            {
                //Fix bug that EEM will cut sting at the equal sign
                Query = Query.Replace("#", "=");

                var result = new StringBuilder();

                using (var con = new SqlConnection(ConfigUtil.GetAppDBConnStr(DB)))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        con.Open();

                        cmd.CommandText = Query;

                        var reader = cmd.ExecuteReader();

                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        if (dt.Rows.Count == 0)
                        {
                            return CmdResult.Failure("Query return no data");
                        }

                        result.AppendLine("Query Result " +Limit +"/"+ dt.Rows.Count);

                        var ct = new ConsoleTable();
                        var _ctCol = new List<string>();
                        foreach (var column in dt.Columns)
                        {
                            _ctCol.Add(column.ToString());
                        }
                        ct.AddColumn(_ctCol);

                        foreach (DataRow dataRow in dt.Rows)
                        {
                            if (Limit-- <= 0)
                            {
                                break;
                            }

                            ct.AddRow(dataRow.ItemArray);
                        }
                        result.AppendLine(ct.ToString());
                        result.AppendLine("===================================");
                        result.AppendLine(Query);
                        result.AppendLine("===================================");
                        return CmdResult.Success(result.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception while running DumpDBCommand: {0}, {1}", ex.Message, Query);
                return CmdResult.Failure(ex.Message);
            }
        }
    }
}
