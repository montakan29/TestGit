using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;


namespace ThomsonReuters.Eikon.SystemTest.Document
{
    public class TestResultParser
    {
        private bool _isValidXml;
        private string _validationError = "";

        public bool ValidXmlDoc(XmlDocument xmlDoc, string targetNamespace, string schemaString)
        {
            var strRdr = new StringReader(schemaString);
            XmlReader xmlRdr = new XmlTextReader(strRdr);
            return ValidXmlDoc(xmlDoc, targetNamespace, xmlRdr);
        }

        public bool ValidXmlDoc(XmlDocument xmlDoc, string targetNamespace, XmlReader xmlRdr)
        {
            _isValidXml = true;
            _validationError = "";
            try
            {
                xmlDoc.Schemas.Add(targetNamespace, xmlRdr);
                var veh = new ValidationEventHandler(ValidationCallBack);
                xmlDoc.Validate(veh);
            }
            catch (Exception ex)
            {
                _isValidXml = false;
                Console.Out.WriteLine("ValidXmlDoc: " + ex.Message);
            }
            return _isValidXml;
        }

        public string ValidationError
        {
            get { return _validationError; }
        }

        private void ValidationCallBack(object sender, ValidationEventArgs args)
        {
            _isValidXml = false;
            _validationError = args.Message;
            Console.Out.WriteLine("ValidationCallBack: " + args.Message);
        }
    }
}
