using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using ThomsonReuters.Eikon.SystemTest.Document;
using ThomsonReuters.Eikon.Toolkit.Interfaces;

namespace ThomsonReuters.Eikon.SystemTestApp
{
    [UiToolkitApp("ThomsonReuters.Eikon.SystemTestApp.WebCallableFunctions")]
    public class WebCalls
    
    {
        public string TSG(string query, string body, IAppServerServices services)
        {
            const string requestAnalyzerServiceXmlns = "http://www.reuters.com/RST/TSG/Service/RequestAnalyzerService";
            var noiseData = GetSoapValue(body, requestAnalyzerServiceXmlns, "UploadNoiseString", "noiseData");
            if (!string.IsNullOrEmpty(noiseData))
            {
                services.AppHits.AppHitsFeatureHit("UP");
                return WebCallsImpl.UploadNoiseString(noiseData, services);
            }

            var dlSize = GetSoapValue(body, requestAnalyzerServiceXmlns, "DownloadNoiseString", "size");
            if (!string.IsNullOrEmpty(dlSize))
            {
                services.AppHits.AppHitsFeatureHit("DOWN");
                int size;
                return int.TryParse(dlSize, out size) ? WebCallsImpl.DownloadNoiseString(size, services) : "";
            }

            var profile1 = GetSoapValue(body, requestAnalyzerServiceXmlns, "RequestHeaders", "profile");
            if (profile1 != null)
            {
                services.AppHits.AppHitsFeatureHit("PROF");
                return WebCallsImpl.RequestHeaders(profile1, services);
            }

            const string getTestSuiteServiceXmlns = "http://www.reuters.com/rst/tsg/";
            var profile = GetSoapValue(body, getTestSuiteServiceXmlns, "GetTestSuite", "profile");
            if (!string.IsNullOrEmpty(profile))
            {
                services.AppHits.AppHitsFeatureHit("TSG");
                return WebCallsImpl.GetTestSuite(profile, services);
            }

            services.Logger.LogWarn("cannot process TSG service: body = " + body);
            return "";
        }

        public string TRR(string query, string body, IAppServerServices services)
        {
            services.AppHits.AppHitsFeatureHit("TRR");
            services.AppHits.AppHitsFeatureHit("");
            var rstResult = GetSoapValue(body, TestResultForwarder.RequestAnalyzerServiceXmlns, "SubmitCompressedTestResult", "rstResult");
            if (!string.IsNullOrEmpty(rstResult)) return WebCallsImpl.SubmitTestResult(rstResult, services);

            rstResult = GetSoapValue(body, TestResultForwarder.RequestAnalyzerServiceXmlns, "SubmitTestResult", "rstResult");
            if (!string.IsNullOrEmpty(rstResult)) return WebCallsImpl.SubmitTestResult(rstResult, services, false);

            //services.Logger.LogWarn("cannot process TRR service: body = " + body);
            return "";
        }

        public string RemoveAllLog(string query, string body, IAppServerServices services)
        {
            /*if (string.IsNullOrEmpty(query)) return "usage: http://AppServer/Apps/SystemTest/RemoveLog?file=All|20151201";
            var fileFormat = query.Split('=');*/
            return WebCallsImpl.RemoveAllLog();
        }

        private static string GetSoapValue(string sXml, string sNs, string sMethod, string data)
        {
            XNamespace xmlNs = sNs;
            sXml = Regex.Replace(sXml, @"[^\u0000-\u007F]", string.Empty);

            try
            {
                var xdoc = XDocument.Parse(sXml);

                if (xdoc.Root != null)
                {
                    var results = from result in xdoc.Descendants(xmlNs + sMethod)
                                  let xElement = result.Element(xmlNs + data)
                                  where xElement != null
                                  select xElement.Value;
                    return results.First();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Invalid XML:" + Environment.NewLine + ex.Message + Environment.NewLine + sXml);
            }
            return null;
        }       
    }
}

