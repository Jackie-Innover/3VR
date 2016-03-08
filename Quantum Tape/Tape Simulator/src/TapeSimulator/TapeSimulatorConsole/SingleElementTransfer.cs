using System;
using System.Threading;

namespace TapeSimulatorConsole
{
    /// <summary>
    ///     A latched transfer of single objects between threads. This is useful for creating pipelines.
    /// </summary>
    public class SingleElementTransfer
    {
        private readonly object _getMonitor = new object();
        private readonly object _putMonitor = new object();
        private object _transferElement;
        //private bool _connectionDisconnected = false;
        public bool IsConnectionDisconnected { get; set; }

        public void Put(object transferElement)
        {
            lock (_putMonitor)
            {
                while (_transferElement != null)
                {
                    Monitor.Wait(_putMonitor);
                }
                _transferElement = transferElement;
            }

            lock (_getMonitor)
            {
                Monitor.PulseAll(_getMonitor);
            }
        }

        public void Reset()
        {
            lock (_getMonitor)
            {
                IsConnectionDisconnected = true;
                Monitor.PulseAll(_getMonitor);
            }
        }

        public object Get()
        {
            object transferElement;

            lock (_getMonitor)
            {
                while (_transferElement == null)
                {
                    if (IsConnectionDisconnected)
                    {
                        Monitor.Wait(_getMonitor, TimeSpan.FromMinutes(1));
                        break;
                    }
                    Monitor.Wait(_getMonitor);
                }
                transferElement = _transferElement;
                _transferElement = null;
            }

            lock (_putMonitor)
            {
                Monitor.PulseAll(_putMonitor);
            }
            return transferElement;
        }
    }
}
