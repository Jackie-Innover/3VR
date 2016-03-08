using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketClient
{
    class Program
    {
        static void Main(string[] args)
        {
            EndPoint endPoint = new DnsEndPoint("127.0.0.1", 8888);
            Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(endPoint);
            byte[] data = new byte[512 * 1024];
            int receiveDataLength = 0;

            while (true)
            {
                receiveDataLength += socket.Receive(data, 0, data.Length, SocketFlags.None);
            }
            Console.ReadKey();
        }
    }
}
