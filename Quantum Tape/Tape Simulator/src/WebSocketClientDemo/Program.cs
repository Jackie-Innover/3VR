using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using BackupLib;
using Polenter.Serialization;
using WebSocket4Net;
using WebSocket4Net.Protocol;

namespace WebSocketClientDemo
{
    class Program
    {
        /**
            The reason why we have to hack origional websocket?

            1. We have to optimize data transfer speed to avoid lost data. (if we do not hack it, the speed is 20MB/s on server version OS, after we hack it, the speed is 350MB/s)
            2. RFC6455 protocol definied: If the data is being sent by the client, the frame(s) MUST be masked. Please refer https://tools.ietf.org/html/rfc6455#section-5.3.
            3. The third party library WebSocket4Net (https://github.com/kerryjiang/WebSocket4Net) do not provide relevant setting to aviode Encode binary data.
            4. Some key classes and proerty are internal or private, we cannot inherit Rfc6455Processor etc.
            5. If we change the source codes of WebSocket4Net, we have to commit source codes of WebSocket4Net to VIMS and manage it.
            6. Make the life to simple, we do not want to manage the source code of WebSocket4Net
        */

        private static long _sendDataCount;

        private static Timer timer = new Timer(state =>
          {
              Console.WriteLine("send data: {0} MB/s", (_sendDataCount / 1024.0 / 1024).ToString("F2"));
              Interlocked.Add(ref _sendDataCount, 0 - _sendDataCount);

          }, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));

        private static readonly AutoResetEvent _autoResetEvent = new AutoResetEvent(true);

        static void superWebSocketInject()
        {
            /** 
                becase m_ProtocolProcessorFactory is a private static field , no associate Property
                and initialized in static WebSocket constructor, 
                so we have to reflect this property and replace origional Rfc6455Process with our hacked rfc6455Processor.
                If you want to check it, please refer WebSocket4Net\WebSocket4Net\WebSocket.cs
            */
            var fieldFactory = typeof(WebSocket).GetField("m_ProtocolProcessorFactory",
                BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.ExactBinding);

            var valFactory = fieldFactory.GetValue(null);

            /**
                because m_OrderedProcessors is a private field of ProtocolProcessorFactory, and initialized in the constructor,
                so have to reflect this property to get the protocol processors.
            */
            var fieldArry = valFactory.GetType()
                .GetField("m_OrderedProcessors",
                BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.ExactBinding);

            var valArry = fieldArry.GetValue(valFactory) as IProtocolProcessor[];
            if (valArry != null)
            {
                for (int i = 0; i < valArry.Length; i++)
                {
                    var processor = valArry[i];
                    if (processor.Version != WebSocketVersion.Rfc6455)
                    {
                        continue;
                    }

                    /**
                        Use Rfc6455Processor by default to transfer data between client and server if not set WebSocketVersion.
                        and our codes did not set WebSocketVersion, So we should hack Rfc6455Processor.
                    */
                    var hackedProcessor = new HackedRfc6455Processor(processor);
                    valArry[i] = hackedProcessor;
                    break;
                }
            }
        }

        static void Main(string[] args)
        {
            superWebSocketInject();
            WebSocket webSocket = new WebSocket("ws://127.0.0.1:24042");
            webSocket.MessageReceived += WebSocket_MessageReceived;
            webSocket.Error += WebSocket_Error;
            webSocket.Closed += WebSocket_Closed;
            webSocket.DataReceived += WebSocket_DataReceived;
            webSocket.Open();
            DataManager dataManager = new DataManager();

            while (true)
            {
                if (webSocket.State == WebSocketState.Open)
                {
                    break;
                }
                Thread.Sleep(1000);
            }
            Console.WriteLine("Connected...");

            //while (true)
            //{
            //    foreach (byte[] packageData in dataManager.PackageDataList)
            //    {
            //        PutFileRequest request = new PutFileRequest
            //        {
            //            ClientGuid = "4C4FEC96-6478-4D25-BEB8-79A68D074E32",
            //            FileData = packageData,
            //            FilePath = "4C4FEC96-6478-4D25-BEB8-79A68D074E32\\2015_01_31\\17\videos\\001\\v_1_20150131_172530_001.avi",
            //            Position = FilePosition.Head
            //        };

            //        var data = WebSocketBlockWrite.PutFileRequestToByte(request);
            //        webSocket.Send(data, 0, data.Length);

            //        //var binaryData = WebSocketBlockWrite.BinaryFormatter(request);
            //        //webSocket.Send(binaryData, 0, binaryData.Length);

            //        //var sharpBinaryData = WebSocketBlockWrite.SharpBinaryFormatter(request);
            //        //webSocket.Send(sharpBinaryData, 0, sharpBinaryData.Length);

            //        //var protoBufData = WebSocketBlockWrite.ProtoBufBinaryFormatter(request);
            //        //webSocket.Send(protoBufData, 0, protoBufData.Length);

            //        //var req = WebSocketBlockWrite.ProtoBufBinaryFormatter(protoBufData);
            //        //Console.WriteLine(req);

            //        //_autoResetEvent.WaitOne();
            //        Interlocked.Add(ref _sendDataCount, packageData.Length);
            //    }
            //}

            Console.ReadKey();
        }

        private static void WebSocket_DataReceived(object sender, DataReceivedEventArgs e)
        {
            _autoResetEvent.Set();
        }

        private static void WebSocket_Closed(object sender, EventArgs e)
        {
            Console.WriteLine("Connection Closed");
            _autoResetEvent.WaitOne();
        }

        private static void WebSocket_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            Console.WriteLine("Error: {0}" + e.Exception);
            _autoResetEvent.WaitOne();
        }

        private static void WebSocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            _autoResetEvent.Set();
        }
    }
}
