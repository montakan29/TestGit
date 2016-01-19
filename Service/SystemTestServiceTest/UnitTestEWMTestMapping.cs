using System.Collections.Generic;
using SystemTestService;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemTestServiceTest
{
    [TestClass]
    public class UnitTestEWMTestMapping
    {
        [TestMethod]
        public void EWMTestMapping()
        {
            var testResult = new TestResult()
            {
                TestID = 1,
                ProdName = "Thomson Reuters Eikon for Wealth Management",
                ProdVers = "1",
                TestCases = new List<TestCase>()
            };

            testResult.TestCases.Add(new TestCase()
            {
                ID = "1",
                Title = "Browser Version:"
            });

            testResult.TestCases.Add(new TestCase()
            {
                ID = "2",
                Title = "Operating System:"
            });

            var list = new List<TestResult> {testResult};

            DataMapper.MapStatCodeEWM(testResult);
            Assert.IsNotNull(testResult);
        }
    }
}
