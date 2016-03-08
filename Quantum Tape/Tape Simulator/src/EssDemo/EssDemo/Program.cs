using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using BackupLib;
using Newtonsoft.Json;
using SuperSocket.SocketBase.Config;
using SuperWebSocket;
using ThreeVR.Libraries.WebSocketBackupLib;

namespace EssDemo
{
    class Program
    {
        private static WebSocketServer socketServer;
        private static long _processDataCount;
        private static WebSocketResponse response = new WebSocketResponse();

        static void Main(string[] args)
        {
            //int port = int.Parse(args[0]);
            int port = 443;
            ServerConfig serverConfig = new ServerConfig
            {
                Name = "SecureSuperWebSocket",
                Ip = "Any",
                Port = port,
                MaxRequestLength = 10485760,
                SendingQueueSize = 500,
                ReceiveBufferSize = 2050 * 1024,
                //SendBufferSize = 600 * 1024,
            };

            socketServer = new WebSocketServer();
            socketServer.NewSessionConnected += SocketServer_NewSessionConnected;
            socketServer.NewDataReceived += SocketServer_NewDataReceived;
            //socketServer.NewMessageReceived += SocketServer_NewMessageReceived;
            socketServer.Setup(serverConfig);
            socketServer.Start();

            Console.WriteLine("Server is running on port {0} ...", port);

            Console.ReadKey();
        }

        private static void SocketServer_NewMessageReceived(WebSocketSession session, string value)
        {
            Console.WriteLine("New Message: {0}", value);
        }

        private static void SocketServer_NewDataReceived(WebSocketSession session, byte[] value)
        {
            try
            {
                var request = WebSocketBlockWrite.ByteToPutFileRequest(value);

                switch (request.RequestType)
                {
                    case WebSocketRequestType.PutFileRequest:

                        PutFileResponse putFileResponse = new PutFileResponse
                        {
                            RequestId = request.RequestId,
                            ExecuteSuccess = true
                        };

                        session.Send(JsonConvert.SerializeObject(putFileResponse));
                        //var data = WebSocketBlockWrite.PutFileResponseToByte(putFileResponse);
                        //session.Send(data, 0, data.Length);

                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void SocketServer_NewSessionConnected(WebSocketSession session)
        {
            var authenticateResponse = new AuthenticationResponse
            {
                ExecuteSuccess = true,
                ServerFeatures = WebSocketServerFeature.Instance.GetFeatures()
            };

            session.Send(JsonConvert.SerializeObject(authenticateResponse));
            Console.WriteLine("Connection accepted");
        }
    }
}
