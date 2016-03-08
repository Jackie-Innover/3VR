using System;
using System.Threading;
using BackupLib;
using SuperSocket.SocketBase.Config;
using SuperWebSocket;

namespace WebSocketServerDemo
{

    class Program
    {
        private static long _processDataCount;
        private static WebSocketResponse response = new WebSocketResponse();
        private static WebSocketServer socketServer;
        private static Timer timer = new Timer(state =>
        {
            Console.WriteLine("Processed data: {0} MB/s", (_processDataCount / 1024.0 / 1024).ToString("F2"));
            Interlocked.Add(ref _processDataCount, 0 - _processDataCount);

        }, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));

        static void Main(string[] args)
        {

            int port = int.Parse(args[0]);
            //int port = 24042;
            ServerConfig serverConfig = new ServerConfig
            {
                Name = "SecureSuperWebSocket",
                Ip = "Any",
                Port = port,
                MaxRequestLength = 10485760,
                SendingQueueSize = 500,
                ReceiveBufferSize = 60 * 1024,
                SendBufferSize = 60 * 1024,
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
            //var putFileRequest = WebSocketBlockWrite.ByteToPutFileRequest(value);
            //Interlocked.Add(ref _processDataCount, putFileRequest.FileData.Length);


            ////var putFileResponse = new PutFileResponse
            ////{
            ////    RequestId = putFileRequest.RequestId,
            ////    ExecuteSuccess = true
            ////};


            //session.Send(JsonConvert.SerializeObject(putFileResponse));

            //session.Send(putFileResponse.ToString());

            #region Send Response

            //Interlocked.Add(ref _processDataCount, value.Length);
            //response.ResponseType = WebSocketResponseType.PutFileResponse;
            ////response.RequestId = putFileRequest.RequestId;
            //response.ExecuteSuccess = true;
            //byte[] responseBytes = WebSocketBlockWrite.ResponseToBytes(response);
            //session.Send(responseBytes, 0, responseBytes.Length);

            #endregion

            #region Signal character

            Interlocked.Add(ref _processDataCount, value.Length);
            session.Send("0");

            #endregion

        }

        private static void SocketServer_NewSessionConnected(WebSocketSession session)
        {
            //var authenticateResponse = new AuthenticationResponse
            //{
            //    ExecuteSuccess = true
            //};            
            response.ResponseType = WebSocketResponseType.AuthenticationResponse;
            response.ExecuteSuccess = true;

            //session.Send(JsonConvert.SerializeObject(authenticateResponse));
            //session.Send(authenticateResponse.ToString());
            byte[] responseBytes = WebSocketBlockWrite.ResponseToBytes(response);
            session.Send(responseBytes, 0, responseBytes.Length);
            Console.WriteLine("Connection accepted");
        }
    }
}
