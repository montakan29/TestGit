using System.Collections.Generic;

namespace SystemTestService
{
    public class Metadata
    {
        public List<TestSchema> testschema { get; set; }
        public List<EnumDict> enumdict { get; set; }
        public List<StatCodeDict> statcodedict { get; set; }
    }

    public class TestSchema
    {
        public string name { get; set; }
        public string type { get; set; }
        public string unit { get; set; }
        public string desc { get; set; }
        public bool enabled { get; set; }
        public bool usedesc { get; set; }
    }

    public class EnumDict
    {
        public List<string> Test { get; set; }
        public List<Map> Map { get; set; }
    }

    public class StatCodeDict
    {
        public List<string> TestName { get; set; }
        public List<Map> Map { get; set; }
    }

    public class Map
    {
        public string ID { get; set; }
        public string Value { get; set; }
        public string Display { get; set; }
    }
}
