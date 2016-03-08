using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using SuperSocket.ClientEngine;

namespace TcpClient
{
    class Program
    {
        private static int _sendedBlockCount;
        static Timer dataTransferMonitor = new Timer(state =>
        {
            int tempSendBlockCount = _sendedBlockCount;
            Interlocked.Exchange(ref _sendedBlockCount, 0);
            Console.WriteLine("Send Data: {0} MB/s", tempSendBlockCount / 2);
        }, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));

        static void Main(string[] args)
        {
            TcpClientSession clientSession= new AsyncTcpSession(new DnsEndPoint("127.0.0.1", 55555));
            clientSession.Connect();
            clientSession.Connected += ClientSession_Connected;
            clientSession.Closed += ClientSession_Closed;
            clientSession.Error += ClientSession_Error;
            clientSession.DataReceived += ClientSession_DataReceived;
            while (true)
            {
                if (clientSession.IsConnected)
                {
                    break;
                }
                Thread.Sleep(1000);
            }
            var data=BlockWriter.GetBytes("abcdefg");
            while (true)
            {
                clientSession.Send(data, 0, data.Length);
                Interlocked.Increment(ref _sendedBlockCount);
            }
            
            Console.ReadKey();
        }

        private static void ClientSession_DataReceived(object sender, DataEventArgs e)
        {
            Interlocked.Increment(ref _sendedBlockCount);
        }

        private static void ClientSession_Error(object sender, ErrorEventArgs e)
        {
            Console.WriteLine(e.Exception);
        }

        private static void ClientSession_Closed(object sender, EventArgs e)
        {
            Console.WriteLine("Closed");
        }

        private static void ClientSession_Connected(object sender, EventArgs e)
        {
            Console.WriteLine("Connected.");
        }
    }
}
