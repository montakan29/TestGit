using System.Collections.Generic;
using SystemTestService;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemTestServiceTest
{
    /// <summary>
    /// Summary description for UnitTestUpdateOpsConosle
    /// </summary>
    [TestClass]
    public class UnitTestUpdateOpsConsole
    {
        public UnitTestUpdateOpsConsole()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void SendStatToOpsConsole()
        {
            var testResult = new TestResult()
            {
                TestID = 1,
                ProdName = "Thomson Reuters Eikon",
                ProdVers = "1",
                UUID = "user1",
                TestCases = new List<TestCase>()
            };

            testResult.TestCases.Add(new TestCase()
            {
                ID = "11",
                Title = "Browser Version:",
                Description = "",
                Value = "",
                Valid = "info"
            });

            testResult.TestCases.Add(new TestCase()
            {
                ID = "9",
                Title = "Operating System:",
                Description = "",
                Value = "",
                Valid = "info"
            });

            testResult.TestCases.Add(new TestCase()
            {
                ID = "24",
                Title = "Computer Name",
                Description = "",
                Value = "",
                Valid = "info"
            });

            var testResult2 = new TestResult()
            {
                TestID = 1,
                ProdName = "Thomson Reuters Eikon for Wealth Management",
                ProdVers = "1",
                UUID = "user2",
                TestCases = new List<TestCase>()
            };

            testResult2.TestCases.Add(new TestCase()
            {
                ID = "1",
                Title = "Browser Version:",
                Description = "",
                Value = "",
                Valid = "info"
            });

            testResult2.TestCases.Add(new TestCase()
            {
                ID = "2",
                Title = "Operating System:",
                Description = "",
                Value = "",
                Valid = "info"
            });

            var testResult3 = new TestResult()
            {
                TestID = 1,
                ProdName = "Thomson Reuters Eikon for Wealth Management",
                ProdVers = "1",
                UUID = "user2",
                TestCases = new List<TestCase>()
            };

            testResult3.TestCases.Add(new TestCase()
            {
                ID = "1",
                Title = "Browser Version:",
                Description = "",
                Value = "",
                Valid = "info"
            });

            testResult3.TestCases.Add(new TestCase()
            {
                ID = "2",
                Title = "Operating System:",
                Description = "",
                Value = "",
                Valid = "info"
            });

            var testList = new List<TestResult> {testResult, testResult2, testResult3};

            var logStatList = UpdateOpsConsoleDb.ConvertToLogStat(testList);
            if (logStatList != null)
            {
                Assert.IsTrue(UpdateOpsConsoleDb.LogAppHits(logStatList));
            }
            else
            {
                Assert.Fail();
            }
        }
    }
}
