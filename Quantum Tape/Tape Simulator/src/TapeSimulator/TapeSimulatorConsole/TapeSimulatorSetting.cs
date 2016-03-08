
using System;
using System.Xml.Linq;
using ThreeVR.Common;

// ReSharper disable PossibleNullReferenceException

namespace TapeSimulatorConsole
{
    public sealed class TapeSimulatorSetting
    {
        #region Properties

        public static readonly TapeSimulatorSetting Instance = new TapeSimulatorSetting();
        public string Host { get; set; }
        public int PortNumber { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public string ClientGuid { get; set; }
        public string ClientDisplayName { get; set; }

        public int SendTimeCount { get; set; }
        public string VideoFilePath { get; set; }
        public int ChannelCount { get; set; }
        public int TransferBlockSize { get; set; }
        public bool IsDirectConnectToEss { get; set; }

        public string Uri;

        public readonly bool IsLoadConfigurationCorrectly;

        #endregion Properties

        #region Life Time

        private TapeSimulatorSetting()
        {
            Uri = string.Format("ws://{0}:{1}", Host, PortNumber);
            try
            {
                var configElement = XElement.Load("TapeSimulatorConfig.xml");
                XElement essRootElement = configElement.Element("ExtendedStorageServerDetails");
                string host = essRootElement.Element("Host").Value;
                string portNumber = essRootElement.Element("Port").Value;
                string userName = essRootElement.Element("UserName").Value;
                string password = essRootElement.Element("Password").Value;
                password = Runtime.GenerateSha1Hash(password);

                XElement tapeSimulatorElement = configElement.Element("TapeSimulatorDetails");

                int sendTimeCount = int.Parse(tapeSimulatorElement.Element("SendTimesPerMinute").Value);
                string videoFilePath = tapeSimulatorElement.Element("VideoFilePath").Value;
                int channelCount;
                if (!int.TryParse(tapeSimulatorElement.Element("ChannelCount").Value, out channelCount))
                {
                    channelCount = 36;
                }

                XElement transferBlockSizeElement = tapeSimulatorElement.Element("TransferBlockSize");
                var transferBlockSize = int.Parse(transferBlockSizeElement.Value);

                XElement isDirectConnectToEssElement = tapeSimulatorElement.Element("IsDirectConnectToESS");
                var isDirectConnectToEss = bool.Parse(isDirectConnectToEssElement.Value);

                Host = host;
                PortNumber = int.Parse(portNumber);
                UserName = userName;
                Password = password;
                //ClientGuid = clientGuid;
                ClientDisplayName = "VMS_TapeSimulator001";

                SendTimeCount = sendTimeCount;
                VideoFilePath = videoFilePath;
                ChannelCount = channelCount;

                TransferBlockSize = transferBlockSize;
                IsDirectConnectToEss = isDirectConnectToEss;

                IsLoadConfigurationCorrectly = true;
            }
            catch (Exception ex)
            {
                IsLoadConfigurationCorrectly = false;
                Console.WriteLine("Fail to load TapeSimulatorConfig.xml, please check it." + ex);
            }
        }

        #endregion Life Time
    }
}