using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.SocketBase;

namespace TcpServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press any key to start the server!");

            Console.ReadKey();
            Console.WriteLine();

            var appServer = new AppServer();
            appServer.NewSessionConnected += AppServer_NewSessionConnected;
            appServer.SessionClosed += AppServer_SessionClosed;
            appServer.NewRequestReceived += AppServer_NewRequestReceived;
            
            //Setup the appServer
            if (!appServer.Setup(55555)) //Setup with listening port
            {
                Console.WriteLine("Failed to setup!");
                Console.ReadKey();
                return;
            }

            Console.WriteLine();

            //Try to start the appServer
            if (!appServer.Start())
            {
                Console.WriteLine("Failed to start!");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("The server started successfully, press key 'q' to stop it!");

            while (Console.ReadKey().KeyChar != 'q')
            {
                Console.WriteLine();
            }

            //Stop the appServer
            appServer.Stop();

            Console.WriteLine("The server was stopped!");
            Console.ReadKey();
        }

        private static void AppServer_NewRequestReceived(AppSession session, SuperSocket.SocketBase.Protocol.StringRequestInfo requestInfo)
        {
            //Console.WriteLine(requestInfo.Key);
        }

        private static void AppServer_SessionClosed(AppSession session, CloseReason value)
        {
            Console.WriteLine("Connection closed.");
        }

        private static void AppServer_NewSessionConnected(AppSession session)
        {
            Console.WriteLine("New connection from {0}", session.RemoteEndPoint);
        }
    }
}
