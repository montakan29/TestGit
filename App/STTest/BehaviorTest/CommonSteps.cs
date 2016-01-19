using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;
using FluentAssertions;
using NUnit.Framework;
using TechTalk.SpecFlow;
using ThomsonReuters.Eikon.SystemTest.Document;
using ThomsonReuters.Eikon.SystemTestApp;

namespace STTest
{
    [Binding]
    public class ParseUnitSteps
    {
        static XmlDocument inputXML = new XmlDocument();
        public static XmlNamespaceManager NamespaceMgr = new XmlNamespaceManager(inputXML.NameTable);
        
        [Given(@"The Testcases are generated from (.*), (.*) and (.*)")]
        public void GivenTheTestcasesAreGeneratedFrom(string statName, string statValue, string statDesc)
        {
            var testCase = new TestCaseExtension {StatName = statName, StatVal = statValue, StatDesc = statDesc};
            ScenarioContext.Current.Set(testCase, "testCase");
        }

        [When(@"Load xml result from (.*)")]
        public void WhenLoadXmlResultFromST_Xml(string xmlFile)
        {
            NamespaceMgr.AddNamespace(TRRConstant.ElementPrefix, TestResultForwarder.DefaultXmlns);
            inputXML.Load(xmlFile);
            var testCase = ScenarioContext.Current.Get<TestCaseExtension>("testCase");

            SetTestCase(ref inputXML, string.Concat("//", TRRConstant.ElementPrefix, ":testCases//", TRRConstant.ElementPrefix, ":testCase"), testCase);
        }


        [When(@"Convert Testcases to TestResult object prepared for json converter")]
        public void WhenConvertTestcasesToTestResultObjectPreparedForJsonConverter()
        {            
            var testResultForwarder = new TestResultForwarder() { XMLResult = inputXML, Logger = TR.AppServer.Logging.Logger.Default };
            var testResult = new TestResult
            {
                Product = TestResultForwarder.ProductName,
                TimeStamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.000Z"),
                UUID = "dummy",
                InstallGuid = "00000000-0000-0000-0000-000000000000",
                MachineGUID = testResultForwarder.GetValueFromXML(string.Concat("//", TRRConstant.ElementPrefix, ":machineID")),
                Stats = testResultForwarder.GetTestCaseListFromXML(string.Concat("//", TRRConstant.ElementPrefix, ":testCases//", TRRConstant.ElementPrefix, ":testCase"))
            };
            var product = TestResultForwarder.ProductName;
            var productName = testResultForwarder.GetValueFromXML(string.Concat("//", TRRConstant.ElementPrefix, ":product/@name"));            
            var productVersion = testResultForwarder.GetValueFromXML(string.Concat("//", TRRConstant.ElementPrefix, ":product/@version"));
            if (TestResultForwarder.IsEWM(productName, productVersion))
            {
                product = "ewm";
                testResultForwarder.MapStatCodeEWM(testResult);
            }
            var runningMode = testResultForwarder.GetValueFromXML(string.Concat("//", TRRConstant.ElementPrefix, ":runningMode"));
            var xnav = inputXML.CreateNavigator();
            var total = Convert.ToInt32(testResultForwarder.GetXPathScalar(xnav, "count(/rst:rstResult/rst:testCases/rst:testCase)", NamespaceMgr));
            var totalPass = Convert.ToInt32(testResultForwarder.GetXPathScalar(xnav, "count(/rst:rstResult/rst:testCases/rst:testCase[not(@shouldBeRerun!='false') and not(@ssiID!='') and rst:valid='pass'])", NamespaceMgr));
            var totalInfo = Convert.ToInt32(testResultForwarder.GetXPathScalar(xnav, "count(/rst:rstResult/rst:testCases/rst:testCase[not(@shouldBeRerun!='false') and not(@ssiID!='') and rst:valid='info'])", NamespaceMgr));
            var totalWarning = Convert.ToInt32(testResultForwarder.GetXPathScalar(xnav, "count(/rst:rstResult/rst:testCases/rst:testCase[not(@shouldBeRerun!='false') and not(@ssiID!='') and rst:valid='warning'])", NamespaceMgr));
            var totalAlert = Convert.ToInt32(testResultForwarder.GetXPathScalar(xnav, "count(/rst:rstResult/rst:testCases/rst:testCase[not(@shouldBeRerun!='false') and not(@ssiID!='') and rst:valid='alert'])", NamespaceMgr));
            var totalFail = Convert.ToInt32(testResultForwarder.GetXPathScalar(xnav, "count(/rst:rstResult/rst:testCases/rst:testCase[not(@shouldBeRerun!='false') and not(@ssiID!='') and rst:valid='fail'])", NamespaceMgr));
            var other = total - totalPass - totalInfo - totalWarning - totalAlert - totalFail;        

            testResult.Stats.Add(new TestCase { StatName = "runmode", StatVal = runningMode });
            testResult.Stats.Add(new TestCase { StatName = "total", StatVal = total.ToString() });
            testResult.Stats.Add(new TestCase { StatName = "pass", StatVal = totalPass.ToString() });
            testResult.Stats.Add(new TestCase { StatName = "info", StatVal = totalInfo.ToString() });
            testResultForwarder.AddValidationStat(xnav, testResult, "warn", totalWarning);
            testResultForwarder.AddValidationStat(xnav, testResult, "alert", totalAlert);
            testResultForwarder.AddValidationStat(xnav, testResult, "fail", totalFail);
            testResult.Stats.Add(new TestCase { StatName = "other", StatVal = other.ToString() });

            var compName = Environment.GetEnvironmentVariable("COMPUTERNAME") ?? "";
            var envName = Environment.GetEnvironmentVariable("AS_ENV") ?? "";

            testResult.Stats.Add(new TestCase { StatName = "servname", StatVal = compName });
            testResult.Stats.Add(new TestCase { StatName = "envname", StatVal = envName });
            //testResult.Stats.Add(new TestCase { StatName = "testID", StatVal = testID });
            testResult.Stats.Add(new TestCase { StatName = "product", StatVal = product });            

            ScenarioContext.Current.Set(testResult, "testResult");
        }

        [Then(@"The test result value for (.*) should be (.*)")]
        public void ThenTheTestResultValueShouldBe(string statName, string expectedValue)
        {
            var testResult = ScenarioContext.Current.Get<TestResult>("testResult");
            var eValule = expectedValue.Split('!');
            
            for (var index = 0; index < statName.Split('!').Length; index++)
            {
                var sName = statName.Split('!')[index];
                foreach (var stat in testResult.Stats.Where(stat => stat.StatName == sName))
                {
                    stat.StatVal.Should().Be(eValule[index]);
                    if (eValule.Length == (index+1))
                        return;
                }
            }

            Assert.Fail();
        }

        public static void SetTestCase(ref XmlDocument doc, string xpath, TestCaseExtension testCase)
        {
            XmlNodeList testCases;
            try
            {
                testCases = doc.SelectNodes(xpath, NamespaceMgr);
            }
            catch (XPathException)
            {
                return;
            }

            foreach (XmlNode testcase in testCases)
            {
                if (testcase.Attributes == null) continue;
                var name = testcase.Attributes["id"].Value.Replace("#test.", "");

                var valueNode = testcase.SelectSingleNode("rst:value", NamespaceMgr);
                var descNode = testcase.SelectSingleNode("rst:description", NamespaceMgr);
                if (valueNode == null || descNode == null) continue;
                valueNode = valueNode.LastChild;
                descNode = descNode.LastChild;
                if (valueNode == null || descNode == null) continue;

                if (testCase.StatName != name) continue;
                valueNode.Value = testCase.StatVal;
                descNode.Value = testCase.StatDesc;
            }
        }

        private static string ReplaceCaseInsensitive(string input, string search, string replacement, bool isEscape = true)
        {
            string result = Regex.Replace(
                input,
                isEscape ? Regex.Escape(search) : search,
                replacement.Replace("$", "$$"),
                RegexOptions.IgnoreCase
            );
            return result;
        }
    }
}
