using System.Collections.Generic;
using System.Linq;

namespace TapeSimulatorConsole
{
    public sealed class ChannelManager
    {
        private readonly List<Channel> _channels = new List<Channel>();

        public static readonly ChannelManager Instance = new ChannelManager();
        private readonly object _lockObject = new object();

        private ChannelManager()
        {
            for (int i = 1; i <= TapeSimulatorSetting.Instance.ChannelCount; i++)
            {
                _channels.Add(new Channel { ApplianceGuid = TapeSimulatorSetting.Instance.ClientGuid, ChannelId = i });
            }
        }

        public Channel GetNextChannel()
        {
            lock (_lockObject)
            {
                Channel channel = _channels.First();
                _channels.Remove(channel);
                _channels.Add(channel);
                return channel;
            }
        }
    }
}