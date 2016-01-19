using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

namespace ThomsonReuters.Eikon.STOpsConsole
{public class HttpFormFileContentPaarser
    {
        public HttpFormFileContentPaarser(string stream, string filePartName)
        {
            this.Parse(stream, filePartName, Encoding.UTF8);
        }

        private void Parse(string body, string filePartName, Encoding encoding)
        {
            this.Success = false;

            // The first line should contain the delimiter
            int delimiterEndIndex = body.IndexOf("\r\n");

            if (delimiterEndIndex > -1)
            {
                string delimiter = body.Substring(0, body.IndexOf("\r\n"));

                string[] parts = body.Split(new string[] { delimiter }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string part in parts)
                {
                    if (part.Contains("Content-Disposition"))
                    {
                        // If we find "Content-Disposition", this is a valid multi-part section
                        // Now, look for the "name" parameter
                        Match nameMatch = new Regex(@"(?<=name\=\"")(.*?)(?=\"")").Match(part);
                        string name = nameMatch.Value.Trim().ToLower();

                        if (name == filePartName)
                        {
                            Regex re = new Regex(@"(?<=Content\-Type:)(.*?)(?=\r\n\r\n)");
                            Match contentTypeMatch = re.Match(part);

                            // Look for filename
                            re = new Regex(@"(?<=filename\=\"")(.*?)(?=\"")");
                            Match filenameMatch = re.Match(part);

                            if (contentTypeMatch.Success && filenameMatch.Success)
                            {
                                // Set properties
                                this.ContentType = contentTypeMatch.Value.Trim();
                                this.Filename = filenameMatch.Value.Trim();
                                // Get the start & end indexes of the file contents
                                int startIndex = contentTypeMatch.Index + contentTypeMatch.Length + "\r\n\r\n".Length;
                                this.FileContents = part.Substring(startIndex);
                            }
                        }

                        if (String.Compare(name, "product", StringComparison.OrdinalIgnoreCase)==0)
                        {
                            //Get product value for this request
                            var re = new Regex(@"(?<=\""product\""\r\n\r\n)(.*?)(?=\r\n)");
                            Match productMatch = re.Match(part);
                            if (productMatch.Success)
                            {
                                this.Product = productMatch.Value.Trim();
                            }
                        }
                    }
                }

                // If some data has been successfully received, set success to true
                if (this.FileContents != null)
                {
                    this.Success = true;
                }
            }
        }

        public bool Success
        {
            get;
            private set;
        }

        public string ContentType
        {
            get;
            private set;
        }

        public string Filename
        {
            get;
            private set;
        }

        public string FileContents
        {
            get;
            private set;
        }

        public string Product
        {
            get;
            private set;
        }
    }
}
