using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using System.Web;
using System.Xml;
using ThomsonReuters.Eikon.SystemTest.Document;
using ThomsonReuters.Eikon.Toolkit.Interfaces;

namespace ThomsonReuters.Eikon.SystemTestApp
{
    public static class WebCallsImpl
    {
        private static string TestSuite = "";
        private static string XSLTString = "";
        private static string TestSuiteFromFile = "";
        private static XmlDocument inputXML;
        private const string TRR_RETURNMSG1 = "<?xml version=\"1.0\" encoding=\"utf-8\"?><soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><soap:Body><SubmitCompressedTestResultResponse xmlns=\"http://www.reuters.com/RST/TRR/Service/TestResultReceiverService\"><SubmitCompressedTestResultResult>&lt;?xml version=\"1.0\" encoding=\"utf-8\"?&gt;&lt;result&gt;&lt;refID&gt;";
        private const string TRR_RETURNMSG2 = "&lt;/refID&gt;&lt;description&gt;Store Succeeded&lt;/description&gt;&lt;/result&gt;</SubmitCompressedTestResultResult></SubmitCompressedTestResultResponse></soap:Body></soap:Envelope>";
        internal static string SubmitTestResult(string rstResult, IAppServerServices services, bool isCompressed = true)
        {
            var uuid = services.UserContext.UUID;
            string testID = "";
            services.Logger.LogInfo("WebCallsImpl.SubmitTestResult(): [" + uuid + "] Started");
            try
            {
                if (isCompressed)
                {                    
                    rstResult = GzipDecompress(rstResult);
                }
                services.Logger.LogDebug("WebCallsImpl.SubmitTestResult(): [" + uuid + "] Decompressed");
                               
                inputXML = LoadStringToXmlDocument(rstResult);

                if (string.IsNullOrEmpty(XSLTString))
                {
                    using (var mStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ThomsonReuters.Eikon.SystemTest.MappingTestResult.xslt"))
                    {
                        using (var reader = new StreamReader(mStream))
                        {
                            mStream.Seek(0, SeekOrigin.Begin);
                            XSLTString = reader.ReadToEnd();
                        }
                    }
                }

                inputXML = FormatingTestResultByXslt(inputXML, XSLTString, services);
                //services.Logger.LogDebug("WebCallsImpl.SubmitTestResult(): [" + uuid + "] XSLT Loaded/Transformed");

                //Insert server detail into test result
                /*#region Insert server detail into the test result
                XmlNode userProfileNode = null;
                XmlNode refNode = inputXML.DocumentElement;
                XmlNode userProfileXml = CreateUserProfileXml(inputXML);
                if (userProfileXml != null)
                {
                    userProfileNode = inputXML.DocumentElement.InsertBefore(userProfileXml, refNode.FirstChild);
                }
                XmlNode serverVersionXml = CreateServerVersionXml(inputXML);
                if (serverVersionXml != null)
                {
                    inputXML.DocumentElement.InsertAfter(serverVersionXml, userProfileNode);
                }
                services.Logger.LogDebug("SubmitTestResult: [" + uuid + "] GetAAAservice");
                #endregion*/

                var forwarder = new TestResultForwarder { XMLResult = inputXML, Logger = services.Logger };
                
                if(!forwarder.ForwardTest(uuid, out testID))
                    SaveOfflineResult(inputXML, uuid);

                services.Logger.LogDebug("WebCallsImpl.SubmitTestResult(): [" + uuid + "] Forwarded");
            }
            catch (Exception ex)
            {
                SaveOfflineResult(inputXML, uuid);
                services.Logger.LogError("SubmitTestResult: [" + uuid + "], " + ex.Message);
                services.Logger.LogError("SubmitTestResult: [" + uuid + "], " + ex.StackTrace);
            }
            if (string.IsNullOrEmpty(testID))
            {               
                return TRR_RETURNMSG1 + uuid + TRR_RETURNMSG2;
            }
            else
            {
                return TRR_RETURNMSG1 + testID + TRR_RETURNMSG2;
            }
            
        }

        internal static string DownloadNoiseString(int size, IAppServerServices services)
        {
            var noiseStringResult = new StringBuilder();
            noiseStringResult.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            noiseStringResult.Append("<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">");
            noiseStringResult.Append("<soap:Body>");
            noiseStringResult.Append("<DownloadNoiseStringResponse xmlns=\"http://www.reuters.com/RST/TSG/Service/RequestAnalyzerService\">");
            noiseStringResult.Append("<DownloadNoiseStringResult>");
            try
            {
                noiseStringResult.Append(size >= 4194304 ? "" : RandomString(size, false));
            }
            catch (Exception)
            {
                noiseStringResult.Append("");
            }
            noiseStringResult.Append("</DownloadNoiseStringResult>");
            noiseStringResult.Append("</DownloadNoiseStringResponse>");
            noiseStringResult.Append("</soap:Body>");
            noiseStringResult.Append("</soap:Envelope>");

            return noiseStringResult.ToString();
        }

        internal static string UploadNoiseString(string noiseData, Toolkit.Interfaces.IAppServerServices services)
        {
            var noiseStringResult = new StringBuilder();
            noiseStringResult.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            noiseStringResult.Append("<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">");
            noiseStringResult.Append("<soap:Body>");
            noiseStringResult.Append("<UploadNoiseStringResponse xmlns=\"http://www.reuters.com/RST/TSG/Service/RequestAnalyzerService\">");
            noiseStringResult.Append("<UploadNoiseStringResult>");
            noiseStringResult.Append(noiseData.Length.ToString());
            noiseStringResult.Append("</UploadNoiseStringResult>");
            noiseStringResult.Append("</UploadNoiseStringResponse>");
            noiseStringResult.Append("</soap:Body>");
            noiseStringResult.Append("</soap:Envelope>");

            return noiseStringResult.ToString();
        }

        private static string RandomString(int size, bool lowerCase)
        {
            var builder = new StringBuilder();
            builder.EnsureCapacity(size);
            var random = new Random();
            for (var i = 0; i < size; i++)
            {
                var ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            return lowerCase ? builder.ToString().ToLower() : builder.ToString();
        }
      
        private static string GzipDecompress(string compressedText)
        {
            var gzipBuffer = Convert.FromBase64String(compressedText);
            var decompressedString = new StringBuilder();
            var writeData = new byte[4096];
            Stream zip = new GZipStream(new MemoryStream(gzipBuffer), CompressionMode.Decompress);
            while (true)
            {
                var size = zip.Read(writeData, 0, writeData.Length);
                if (size > 0)
                {
                    decompressedString.Append(Encoding.Unicode.GetString(writeData, 0, size));
                }
                else
                {
                    break;
                }
            }
            zip.Close();
            return decompressedString.ToString();
        }

        private static XmlDocument LoadStringToXmlDocument(string xmlString)
        {
            xmlString = "" + xmlString;
            if (xmlString.Equals(""))
            {
                //log.Warn("Invalid RST XML Result");
                //log.Log(GMIErrorMsgList.TRR001);
                throw new Exception("Invalid System Test XML Result");
            }

            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlString);
                return xmlDoc;
            }
            catch (XmlException ex)
            {
                //log.Warn(ex.Message);
                //log.Log(GMIErrorMsgList.TRR001);
                throw new Exception(ex.Message);
            }
        }

        private static XmlDocument FormatingTestResultByXslt(XmlDocument doc, string xsltString, IAppServerServices services)
        {
            try
            {
                var formattedXml = new XmlDocument();
                var xdelc = formattedXml.CreateXmlDeclaration("1.0", "utf-8", null);
                formattedXml.AppendChild(xdelc);

                var formattedXmlString = new StringBuilder();
                var settings = new XmlWriterSettings
                {
                    Encoding = new UTF8Encoding(false),
                    ConformanceLevel = ConformanceLevel.Document,
                    Indent = false
                };
                var writer = XmlWriter.Create(formattedXmlString, settings);

                XmlReader xmlreader = new XmlTextReader(new StringReader(xsltString));
                var doXslt = new System.Xml.Xsl.XslCompiledTransform();
                doXslt.Load(xmlreader);
                doXslt.Transform(doc, writer);
                writer.Close();
                xmlreader.Close();

                var tmp = new XmlDocument();
                tmp.LoadXml(formattedXmlString.ToString());

                var xe = formattedXml.ImportNode(tmp.DocumentElement, true);
                formattedXml.AppendChild(xe);
                return formattedXml;
            }
            catch (Exception ex)
            {
                services.Logger.LogError("WebCallsImpl.FormatingTestResultByXslt(): " + ex.Message);
                return doc;
            }
        }

        /*private static XmlNode CreateUserProfileXml(XmlDocument xmlDoc)
        {
            try
            {
                UserProfile userProfile = null;
                AccountHierarchyInfo userAccountHierachyInfo = null;

                string uuid = "";
                XmlNamespaceManager namespaceMngr = new XmlNamespaceManager(xmlDoc.Schemas.NameTable);
                namespaceMngr.AddNamespace("rst", TRRConstant.DefaultNamespace);
                XmlNode foundUUID = xmlDoc.SelectSingleNode("/rst:rstResult/rst:UUIDTest", namespaceMngr);
                if (foundUUID == null)
                {
                    uuid = this.getUUIDFromHeader();
                }
                else
                {
                    uuid = foundUUID.InnerText;
                    xmlDoc.DocumentElement.RemoveChild(foundUUID);
                }

                if (uuid == null || uuid.Equals(""))
                {
                    //log.Debug("Invalid UUID");
                    //log.Log(GMIErrorMsgList.TRR011);
                    throw new Exception("TR04_1: Invalid UUID");
                }

                this.initialUserAccount(uuid, ref userProfile, ref userAccountHierachyInfo);

                XmlNode newNode = null;
                XmlNode docNode = xmlDoc.CreateElement(String.Empty, "userProfile", TRRConstant.DefaultNamespace);

                newNode = docNode.AppendChild(xmlDoc.CreateElement(String.Empty, "UUID", TRRConstant.DefaultNamespace));
                newNode.AppendChild(xmlDoc.CreateTextNode(uuid));

                if (userProfile != null)
                {
                    newNode = docNode.AppendChild(xmlDoc.CreateElement(String.Empty, "accountDomain", TRRConstant.DefaultNamespace));
                    newNode.AppendChild(xmlDoc.CreateTextNode(userProfile.AccountDomain));
                    newNode = docNode.AppendChild(xmlDoc.CreateElement(String.Empty, "userName", TRRConstant.DefaultNamespace));
                    newNode.AppendChild(xmlDoc.CreateTextNode(userProfile.UserID));
                    newNode = docNode.AppendChild(xmlDoc.CreateElement(String.Empty, "email", TRRConstant.DefaultNamespace));
                    newNode.AppendChild(xmlDoc.CreateTextNode(userProfile.Email));
                    newNode = docNode.AppendChild(xmlDoc.CreateElement(String.Empty, "contactName", TRRConstant.DefaultNamespace));
                    newNode.AppendChild(xmlDoc.CreateTextNode(string.Concat(userProfile.FirstName, " ", userProfile.LastName)));
                    //if (userProfile.Country != null) {
                    //    newNode = docNode.AppendChild(xmlDoc.CreateElement(String.Empty, "geoCountry", TRRConstant.DefaultNamespace));
                    //    newNode.AppendChild(xmlDoc.CreateTextNode(userProfile.Country));
                    //}
                }
                if (userAccountHierachyInfo != null)
                {
                    newNode = docNode.AppendChild(xmlDoc.CreateElement(String.Empty, "location", TRRConstant.DefaultNamespace));
                    ((XmlElement)newNode).SetAttribute("CID", userAccountHierachyInfo.Location.AccountId);
                    newNode.AppendChild(xmlDoc.CreateTextNode(userAccountHierachyInfo.Location.AccountName));

                    newNode = docNode.AppendChild(xmlDoc.CreateElement(String.Empty, "country", TRRConstant.DefaultNamespace));
                    ((XmlElement)newNode).SetAttribute("CID", userAccountHierachyInfo.LegalEntity.AccountId);
                    newNode.AppendChild(xmlDoc.CreateTextNode(userAccountHierachyInfo.LegalEntity.AccountName));

                    newNode = docNode.AppendChild(xmlDoc.CreateElement(String.Empty, "global", TRRConstant.DefaultNamespace));
                    ((XmlElement)newNode).SetAttribute("CID", userAccountHierachyInfo.UltimateParent.AccountId);
                    newNode.AppendChild(xmlDoc.CreateTextNode(userAccountHierachyInfo.UltimateParent.AccountName));
                }
                if (userProfile != null)
                {
                    if (userProfile.Country != null)
                    {
                        newNode = docNode.AppendChild(xmlDoc.CreateElement(String.Empty, "geoCountry", TRRConstant.DefaultNamespace));
                        newNode.AppendChild(xmlDoc.CreateTextNode(userProfile.Country));
                    }
                }
                return docNode;
            }
            catch
            {
                throw;
            }
        }

        private static XmlNode CreateServerVersionXml(XmlDocument xmlDoc)
        {
            try
            {
                var assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName(false).Name;
                var assemblyVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName(false).Version.ToString();
                var compName = Environment.MachineName;

                XmlNode docNode = xmlDoc.CreateElement(String.Empty, "server", TRRConstant.DefaultNamespace);
                docNode.Attributes.Append(xmlDoc.CreateAttribute("name")).Value = assemblyName;
                docNode.Attributes.Append(xmlDoc.CreateAttribute("version")).Value = assemblyVersion;
                docNode.Attributes.Append(xmlDoc.CreateAttribute("compName")).Value = compName;

                var confDataCenter = string.Concat("", Env.CurrentEnv);
                XmlNode additionNode = null;
                additionNode = xmlDoc.CreateElement(String.Empty, "dataCenter", TRRConstant.DefaultNamespace);
                additionNode.AppendChild(xmlDoc.CreateTextNode(confDataCenter));
                docNode.AppendChild(additionNode);

                DateTime wasDateTime = DateTime.Now;
                string wasDTstring = wasDateTime.ToString("yyyy-MM-ddTHH:mm:sszz:00");
                additionNode = xmlDoc.CreateElement(String.Empty, "wasDateTime", TRRConstant.DefaultNamespace);
                additionNode.AppendChild(xmlDoc.CreateTextNode(wasDTstring));
                docNode.AppendChild(additionNode);

                return docNode;
            }
            catch
            {
                throw;
            }
        }
        */
        private static void SaveOfflineResult(XmlDocument offlineResult, string uuid)
        {
            if (offlineResult == null) return;
            try
            {
                var fileName = string.Format("{0}-{1:yyyyMMdd}.xml", uuid, DateTime.Now);
                var offlineResultsFolderPath = Path.Combine(Environment.ExpandEnvironmentVariables("%AS_DATA%"), "RSTStat\\OfflineResults\\");
                if (!Directory.Exists(offlineResultsFolderPath))
                {
                    Directory.CreateDirectory(offlineResultsFolderPath);
                }

                offlineResult.Save(Path.Combine(offlineResultsFolderPath, fileName));

            }
            catch (Exception)
            {
            }
        }
        
        // simple the function that only called by Eikon 3.x by return static TestSuite => do not calculate from profile
        internal static string GetTestSuite(string profile, IAppServerServices services)
        {
            if (!string.IsNullOrEmpty(TestSuite)) return TestSuite;            
            try
            {
                if (string.IsNullOrEmpty(TestSuiteFromFile))
                {
                    using (var xStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ThomsonReuters.Eikon.SystemTest.TestSuite.xml"))
                    {
                        using (var reader = new StreamReader(xStream))
                        {
                            xStream.Seek(0, SeekOrigin.Begin);
                            TestSuiteFromFile = reader.ReadToEnd();
                        }
                    }
                }

                var testSuiteReturn = new StringBuilder();
                testSuiteReturn.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                testSuiteReturn.Append("<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">");
                testSuiteReturn.Append("<soap:Body>");
                testSuiteReturn.Append("<GetTestSuiteResponse xmlns=\"http://www.reuters.com/rst/tsg/\">");
                testSuiteReturn.Append("<GetTestSuiteResult>");
                testSuiteReturn.Append(HttpUtility.HtmlEncode(TestSuiteFromFile));
                testSuiteReturn.Append("</GetTestSuiteResult>");
                testSuiteReturn.Append("</GetTestSuiteResponse>");
                testSuiteReturn.Append("</soap:Body>");
                testSuiteReturn.Append("</soap:Envelope>");
                TestSuite = testSuiteReturn.ToString();
            }
            catch (Exception e)
            {
                services.Logger.LogWarn("WebCallsImpl.GetTestSuite(): Cannot load TestSuite, error = " + e.Message);
                return "";
            }

            services.Logger.LogDebug("WebCallsImpl.GetTestSuite(): TestSuite = " + TestSuite);
            return TestSuite;
        }

        internal static string RequestHeaders(string profile1, IAppServerServices services)
        {
            try
            {
                var sbHeaders = new StringBuilder();
                sbHeaders.Append("POST /SystemTest/TSG/RequestAnalyzerService.asmx HTTP/1.1\n");
                sbHeaders.Append("Cache-Control: no-cache\n");
                sbHeaders.Append("Connection: Keep-Alive\n");
                sbHeaders.Append("Content-Length: 393\n");
                sbHeaders.Append("Content-Type: text/xml; charset=utf-8\n");
                sbHeaders.Append("Accept-Encoding: gzip, deflate\n");
                sbHeaders.Append("Authorization: dsame\n");
                sbHeaders.Append("Cookie: EIKON_USER_AGENT='NET40,EIKON8.3.503,SR0,UA12.1.34,ADF6.20132.5.31,Charcoal'; iPlanetDirectoryPro=\n"); // blank the iPlanetDirectoryPro value
                sbHeaders.Append("Host: \n"); // blank the request URL
                sbHeaders.Append("User-Agent: Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; InfoPath.3; .NET4.0C; .NET4.0E; Thomson Reuters File Download 6.0)\n");
                sbHeaders.Append("SOAPAction: http://www.reuters.com/RST/TSG/Service/RequestAnalyzerService/RequestHeaders\n");
                sbHeaders.Append("reutersuuid: " + services.UserContext.UUID);
                var strHeader = sbHeaders.ToString();
                services.Logger.LogDebug("WebCallsImpl.RequestHeaders(): strHeader = " + strHeader);
                return strHeader;
            }
            catch (Exception e)
            {
                services.Logger.LogWarn("WebCallsImpl.RequestHeaders(): error = " + e.Message);
                return "";
            }
        }

        internal static string RemoveAllLog()
        {
            try
            {
                var offlineResultsFolderPath = Path.Combine(Environment.ExpandEnvironmentVariables("%AS_DATA%"),
                    "RSTStat\\OfflineResults\\");
                Array.ForEach(Directory.GetFiles(offlineResultsFolderPath), File.Delete);

            }
            catch (Exception e)
            {
                return "error, " + e.Message;
            }
            return "done";
        }
    }
}
