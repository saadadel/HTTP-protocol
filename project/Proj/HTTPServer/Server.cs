using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {
        Socket serverSocket;

        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            this.LoadRedirectionRules(redirectionMatrixPath);
            //TODO: initialize this.serverSocket
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint IPEndPoint = new IPEndPoint(IPAddress.Any, portNumber);
            serverSocket.Bind(IPEndPoint);
        }

        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.
            serverSocket.Listen(100);

            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                //TODO: accept connections and start thread for each accepted connection.
                Socket newClient = serverSocket.Accept();
                //Console.WriteLine("New client accepted: {0}", newClient.RemoteEndPoint);
                Thread thread = new Thread(new ParameterizedThreadStart(HandleConnection));
                thread.Start(newClient);
            }
        }

        public void HandleConnection(object obj)
        {
            // TODO: Create client socket 
            Socket theClient = (Socket)obj;
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            theClient.ReceiveTimeout = 0;
            
            // TODO: receive requests in while true until remote client closes the socket.
            while (true)
            {
                try
                {
                    // TODO: Receive request
                    byte[] dataReceived = new byte[1024 * 1024];
                    int dataReceivedLen = theClient.Receive(dataReceived);

                    // TODO: break the while loop if receivedLen==0
                    if (dataReceivedLen == 0) { break; }

                    // TODO: Create a Request object using received request string
                    string receivedRequest = Encoding.ASCII.GetString(dataReceived, 0, dataReceivedLen);
                    Request request = new Request(receivedRequest);

                    // TODO: Call HandleRequest Method that returns the response
                    Response response = HandleRequest(request);

                    // TODO: Send Response back to client
                    theClient.Send(Encoding.ASCII.GetBytes(response.ResponseString));

                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                }
            }

            // TODO: close client socket
        }

        Response HandleRequest(Request request)
        {
            //throw new NotImplementedException();
            Response response;
            StatusCode statusCode;
            string codeName;
            string content;
            string redirect;
            try
            {
                //TODO: check for bad request 
                bool parseRequest = request.ParseRequest();
                if (!parseRequest)
                {
                    codeName = "BadRequest";
                    content = LoadDefaultPage(Configuration.BadRequestDefaultPageName);
                    redirect = "";
                }
                else
                {
                    //TODO: map the relativeURI in request to get the physical path of the resource.
                    string physicalURI = Configuration.RootPath + request.relativeURI;

                    //TODO: check for redirect
                    //Console.WriteLine("request.relativeURI: " + request.relativeURI);
                    redirect = GetRedirectionPagePathIFExist(request.relativeURI);
                    if (!String.IsNullOrEmpty(redirect))
                    {
                        codeName = "Redirect";
                        redirect = redirect.Insert(0, "/");
                        content = LoadDefaultPage(redirect);
                    }
                    else
                    {
                        //TODO: check file exists
                        if (!File.Exists(physicalURI))
                        {
                            codeName = "NotFound";
                            content = LoadDefaultPage(Configuration.NotFoundDefaultPageName);
                        }
                        else
                        {
                            //TODO: read the physical file
                            content = LoadDefaultPage(request.relativeURI);
                            codeName = "OK";
                        }
                        
                    }


                    

                    
                }


                bool state = Enum.TryParse(codeName, out statusCode);
                // Create OK response
                response = new Response(statusCode, "text/html", content, redirect);
                return response;
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                // TODO: in case of exception, return Internal Server Error.
                codeName = "InternalServerError";
                bool state = Enum.TryParse(codeName, out statusCode);
                content = LoadDefaultPage(Configuration.InternalErrorDefaultPageName);
                response = new Response(statusCode, "text/html", content, null);
                return response;
            }
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            //Console.WriteLine(relativePath);
            relativePath = relativePath.Remove(0, 1);
            if (Configuration.RedirectionRules.ContainsKey(relativePath))
            {
                //header_lines += request.relativeURI + ": " + Configuration.RedirectionRules[request.relativeURI];
                return Configuration.RedirectionRules[relativePath];
            }
            return string.Empty;
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Configuration.RootPath + defaultPageName;
            // TODO: check if filepath not exist log exception using Logger class and return empty string
            if (File.Exists(filePath))
            {
                string text = System.IO.File.ReadAllText(filePath);
                return text;
            }

            // else read file and return its content
            return string.Empty;
        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file 
                // then fill Configuration.RedirectionRules dictionary 
                Configuration.RedirectionRules = new Dictionary<string, string>();
                string[] lines = System.IO.File.ReadAllLines(filePath);
                foreach(string line in lines)
                {
                    string[] linedata = line.Split(',');
                    Configuration.RedirectionRules.Add(linedata[0], linedata[1]);
                }
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                Environment.Exit(1);
            }
        }
    }
}
