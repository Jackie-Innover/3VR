using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DuplicateSocketServer
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpListener tcpListener = new TcpListener(IPAddress.Any, 9999);
            tcpListener.Start();
            Socket socket = tcpListener.AcceptSocket();
            Console.WriteLine("New Connection from {0}", socket.LocalEndPoint);
            Console.ReadKey();
        }
    }
}
