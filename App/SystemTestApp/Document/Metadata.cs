using System.Collections.Generic;

namespace ThomsonReuters.Eikon.SystemTest.Document
{
    public class Metadata
    {
        public List<TestSchema> Testschema { get; set; }
        public List<EnumDict> EnumDict { get; set; }
        public List<StatCodeDict> StatCodeDict { get; set; }
    }

    public class TestSchema
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Unit { get; set; }
        public string Desc { get; set; }
        public bool Enabled { get; set; }
        public bool Usedesc { get; set; }
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
