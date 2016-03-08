using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using ThreeVR.Libraries.Common;

namespace TapeSimulatorConsole
{
    public static class SendManager
    {
        private static readonly QueuingThreadPool _threadPool = new QueuingThreadPool("TapeSimulator", 1);
        private static readonly WebSocketHandler webSocketHandler = new WebSocketHandler();
        private static bool _running;

        public static void SendData()
        {
            while (true)
            {
                _running = true;
                _threadPool.AddTask(SendDataToEss);
                Thread.Sleep(60 * 1000);

                while (_running)
                {
                    Console.WriteLine("Warnning: Not able to send all scheduled files in 1 min, still waiting");
                    Thread.Sleep(1000);
                }
            }
        }

        private static void SendDataToEss()
        {
            WebSocketClient.MonitorSendDataCounTimer.Change(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
            Stopwatch stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < TapeSimulatorSetting.Instance.SendTimeCount; i++)
            {
                Channel channel = ChannelManager.Instance.GetNextChannel();
                string videoFilePath = channel.NextFileName();
                webSocketHandler.PutFile(videoFilePath);
            }

            stopwatch.Stop();
            if (stopwatch.Elapsed.TotalSeconds > 60)
            {
                Console.WriteLine("Warnning: Send over time!!!. Spent time:{0}s", stopwatch.Elapsed.TotalSeconds);
            }
            else
            {
                Console.WriteLine("Send done. Spent time {0}s", stopwatch.Elapsed.TotalSeconds.ToString("F2"));
            }
            WebSocketClient.MonitorSendDataCounTimer.Change(Timeout.Infinite, Timeout.Infinite);
            WebSocketClient.ResetSendDataCount();

            FileInfo fileInfo = new FileInfo(TapeSimulatorSetting.Instance.VideoFilePath);
            double sendSpeed = (fileInfo.Length * 1.0 * TapeSimulatorSetting.Instance.SendTimeCount /
                                stopwatch.Elapsed.TotalSeconds / 1024 / 1024);
            Console.WriteLine("Average send speed: {0} MB/s", sendSpeed.ToString("F2"));

            _running = false;
        }
    }
}