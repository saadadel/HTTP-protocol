using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        string[] requestLines;
        public HTTPVersion httpVersion;
        RequestMethod method;
        public string relativeURI;
        Dictionary<string, string> headerLines;
        string requestString;
        string[] contentLines;
        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        public Request(string requestString)
        {
            this.requestString = requestString;
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest()
        {
            //throw new NotImplementedException();

            //TODO: parse the receivedRequest using the \r\n delimeter
            requestLines = requestString.Split("\n".ToCharArray());

            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            if(requestLines.Length < 4) { return false; }
            // Parse Request line
            bool checkparse = ParseRequestLine();
            if (!checkparse) { return false; }

            // Validate blank line exists
            bool checkvalidation = ValidateBlankLine();
            if (!checkvalidation) { return false; }

            // Load header lines into HeaderLines dictionary
            bool checkheaders = LoadHeaderLines();
            if (!checkheaders) { return false; }

            return true;
        }

        private bool ParseRequestLine()
        {
            //throw new NotImplementedException();

            string[] requestLine = requestLines[0].Split(' ');

            bool setMethod = Enum.TryParse(requestLine[0], out method);
            if (!setMethod) { return false; }


            bool testURI = ValidateIsURI(requestLine[1]);
            if(!testURI) { return false; }
            relativeURI = requestLine[1];

            //requestLine[2] = requestLine[2].Remove(requestLine[2].Length - 1, 2);
            //bool setVersion = Enum.TryParse(requestLine[2], out httpVersion);
            if (requestLine[2].Contains("HTTP/1.1")) { httpVersion = HTTPVersion.HTTP11; }
            else if (requestLine[2].Contains("HTTP/1.0")) { httpVersion = HTTPVersion.HTTP10; }
            else if (requestLine[2].Contains("HTTP/0.9")) { httpVersion = HTTPVersion.HTTP09; }
            else { return false; }

            return true;
        }

        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool LoadHeaderLines()
        {
            //throw new NotImplementedException();
            int index = 1;
            if (String.IsNullOrEmpty(requestLines[index])) { return false; }

            //Console.WriteLine("ArrSize: " + requestLines.Length);
            /*Console.WriteLine("___________________________________________");
            for (int i = 0; i<requestLines.Length; i++)
            {
                Console.WriteLine(i + "     " +  requestLines[i]);
            }
            Console.WriteLine("___________________________________________");*/
            headerLines = new Dictionary<string, string>();
            string[] header;
            char[] seperator = ":".ToCharArray();
            while (!String.IsNullOrEmpty(requestLines[index]) && requestLines[index]!="" && requestLines[index] != "\n" && requestLines[index] != "\r\n" && requestLines[index] != null && requestLines[index] != String.Empty && requestLines[index] != Environment.NewLine && requestLines[index] != "\r")
            {
                header = requestLines[index].Split(seperator, 2);
                //Console.WriteLine("reqeustLine: " + requestLines[index]);

                //Console.WriteLine(header[1]);
                header[1] = header[1].Remove(0, 1);
                /*Console.WriteLine("header length: " + header.Length);
                Console.WriteLine(header[0]);
                Console.WriteLine(header[1]);
                Console.WriteLine("_______________________");*/
                headerLines.Add(header[0], header[1]);
                //Console.WriteLine("INDEX: " + index);
                index++;
            }
            return true;
        }

        private bool ValidateBlankLine()
        {
            //throw new NotImplementedException();
            if (requestLines[requestLines.Length - 2] == "\r") { return true; }
            else { return false; }
        }

    }
}
