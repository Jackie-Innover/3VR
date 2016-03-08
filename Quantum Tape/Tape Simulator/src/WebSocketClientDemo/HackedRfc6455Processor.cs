using System;
using System.Collections.Generic;
using System.Reflection;
using SuperSocket.ClientEngine;
using WebSocket4Net;
using WebSocket4Net.Protocol;

namespace WebSocketClientDemo
{
    class HackedRfc6455Processor : IProtocolProcessor
    {
        private IProtocolProcessor m_baseProcessor;
        private PropertyInfo m_webSocketClientProp;

        private static object[] _0SizedObjectArry = new object[0];

        public HackedRfc6455Processor(IProtocolProcessor baseObj)
        {
            m_baseProcessor = baseObj;

            m_webSocketClientProp = typeof(WebSocket).GetProperty("Client", BindingFlags.Instance | BindingFlags.ExactBinding | BindingFlags.GetProperty | BindingFlags.NonPublic);
        }

        private TcpClientSession getClientSession(WebSocket websocket)
        {
            return m_webSocketClientProp.GetValue(websocket, _0SizedObjectArry) as TcpClientSession;
        }

        private byte[] hackedEncodeDataFrame(int opCode, byte[] playloadData, int offset, int length)
        {
            int masklen = 0;
            byte[] numArray;
            if (length < 126)
            {
                numArray = new byte[2 + masklen + length];
                numArray[1] = (byte)length;
            }
            else if (length < 65536)
            {
                numArray = new byte[4 + masklen + length];
                numArray[1] = (byte)126;
                numArray[2] = (byte)(length / 256);
                numArray[3] = (byte)(length % 256);
            }
            else
            {
                numArray = new byte[10 + masklen + length];
                numArray[1] = (byte)127;
                int num2 = length;
                int num3 = 256;
                for (int index = 9; index > 1; --index)
                {
                    numArray[index] = (byte)(num2 % num3);
                    num2 /= num3;
                    if (num2 == 0)
                        break;
                }
            }
            numArray[0] = (byte)(opCode | 0x80);

            int dstOffset = numArray.Length - length;

            if (length > 0)
                Buffer.BlockCopy(playloadData, offset, numArray, dstOffset, length);
            return numArray;
        }

        public bool SupportBinary
        {
            get { return m_baseProcessor.SupportBinary; }
        }

        public bool SupportPingPong
        {
            get { return m_baseProcessor.SupportPingPong; }
        }

        public ICloseStatusCode CloseStatusCode
        {
            get { return m_baseProcessor.CloseStatusCode; }
        }

        public WebSocketVersion Version
        {
            get { return m_baseProcessor.Version; }
        }

        public void SendHandshake(WebSocket websocket)
        {
            m_baseProcessor.SendHandshake(websocket);
        }

        public bool VerifyHandshake(WebSocket websocket, WebSocketCommandInfo handshakeInfo, out string description)
        {
            return m_baseProcessor.VerifyHandshake(websocket, handshakeInfo, out description);
        }

        public ReaderBase CreateHandshakeReader(WebSocket websocket)
        {
            return m_baseProcessor.CreateHandshakeReader(websocket);
        }

        public void SendMessage(WebSocket websocket, string message)
        {
            m_baseProcessor.SendMessage(websocket, message);
        }

        public void SendData(WebSocket websocket, byte[] data, int offset, int length)
        {
            var client = getClientSession(websocket);
            if (client == null)
            {
                return;
            }

            //this.SendDataFragment(websocket, 2, data, offset, length);
            //byte[] data = this.EncodeDataFrame(opCode, playloadData, offset, length);
            //websocket.Client.Send(data, 0, data.Length);

            /**
                Do not encode data frame, remove mask and mask key.
            */
            byte[] encodedData = hackedEncodeDataFrame(OpCode.Binary, data, offset, length);
            client.Send(encodedData, 0, encodedData.Length);
        }

        public void SendData(WebSocket websocket, IList<ArraySegment<byte>> segments)
        {
            //if (!this.EnsureWebSocketOpen())
            //    return;
            //this.ProtocolProcessor.SendData(this, segments);

            var client = getClientSession(websocket);
            if (client == null)
            {
                return;
            }

            //List<ArraySegment<byte>> list = new List<ArraySegment<byte>>(segments.Count);
            //for (int index = 0; index < segments.Count; ++index)
            //{
            //    ArraySegment<byte> arraySegment = segments[index];
            //    list.Add(new ArraySegment<byte>(this.EncodeDataFrame(2, arraySegment.Array, 0, arraySegment.Count)));
            //}
            //websocket.Client.Send((IList<ArraySegment<byte>>)list);
            List<ArraySegment<byte>> list = new List<ArraySegment<byte>>(segments.Count);
            for (int index = 0; index < segments.Count; ++index)
            {
                ArraySegment<byte> arraySegment = segments[index];
                list.Add(new ArraySegment<byte>(hackedEncodeDataFrame(OpCode.Binary, arraySegment.Array, 0, arraySegment.Count)));
            }

            client.Send(list);
        }

        public void SendCloseHandshake(WebSocket websocket, int statusCode, string closeReason)
        {
            m_baseProcessor.SendCloseHandshake(websocket, statusCode, closeReason);
        }

        public void SendPing(WebSocket websocket, string ping)
        {
            m_baseProcessor.SendPing(websocket, ping);
        }

        public void SendPong(WebSocket websocket, string pong)
        {
            m_baseProcessor.SendPong(websocket, pong);
        }
    }
}
