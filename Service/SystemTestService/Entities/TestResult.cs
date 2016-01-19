using System;
using System.Collections.Generic;

namespace SystemTestService
{
    public class TestResult
    {
        public String UUID{get;set;}
        public String CapbName{get;set;}
        public String CapbVers { get; set; }
        public String CapbServer { get; set; }
        public String ProdName { get; set; }
        public String ProdVers { get; set; }
        public String ProdComponentName { get; set; }
        public String ProdComponentVers { get; set; }
        public int TestID { get; set; }
        public String UserProfileUUID { get; set; }
        public String UserProfileAccountDomain { get; set; }
        public String UserProfileUserName { get; set; }
        public String UserProfileEmail { get; set; }
        public String UserProfileContactName { get; set; }
        public String UserProfileLocation { get; set; }
        public String UserProfileCID { get; set; }
        public String UserProfileGeoCountry { get; set; }
        public String ServerName { get; set; }
        public String ServerVersion { get; set; }
        public String ServerCompName { get; set; }
        public String ServerDatacenter { get; set; }
        public DateTime ServerWasDateTime { get; set; }
        public DateTime ServerDbsDateTime { get; set; }
        public String ClientName { get; set; }
        public String ClientVersion { get; set; }
        public String MachineID { get; set; }
        public String RunningMode { get; set; }
        public DateTime LocalDateTime { get; set; }
        public List<TestCase> TestCases { get; set; }
    }

    public class TestCase
    {
        public String ID { get; set; }
        public String Title { get; set; }
        public String Group { get; set; }
        public String Valid { get; set; }
        public String Value { get; set; }
        public String Description { get; set; }
        public String Recommendation { get; set; }
        public String RphURL { get; set; }
        public String ResultID { get; set; }
    }
}
