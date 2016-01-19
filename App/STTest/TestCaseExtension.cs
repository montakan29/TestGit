using System;
using ThomsonReuters.Eikon.SystemTest.Document;

namespace STTest
{
    public class TestCaseExtension : TestCase
    {
        public String StatDesc { get; set; }
        public String ExpectedValue { get; set; }
    }
}
