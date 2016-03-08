using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketDuplicateAndClose
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpListener tcpListener = new TcpListener(IPAddress.Any, 8888);
            tcpListener.Start();
            Socket socket = tcpListener.AcceptSocket();
            SocketInformation socketInformation = socket.DuplicateAndClose(9212);
            //Socket newSocket=new Socket(socketInformation);
            //newSocket.

            TcpClient tcpClient = new TcpClient();
            tcpClient.Connect("127.0.0.1", 9999);
            NetworkStream stream = tcpClient.GetStream();
            stream.Write();

            Console.ReadKey();
        }
    }
}
