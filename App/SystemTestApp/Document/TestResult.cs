using System;
using System.Collections.Generic;

namespace ThomsonReuters.Eikon.SystemTest.Document
{
    public class TestResult
    {        
        public String Product { get; set; }
        public String TimeStamp { get; set; }
        public String UUID { get; set; }
        public String InstallGuid { get; set; }
        public String MachineGUID { get; set; }

        public List<TestCase> Stats { get; set; }        
    }

    public class TestCase
    {
        public String StatName { get; set; }
        public String StatVal { get; set; }
    }
}
