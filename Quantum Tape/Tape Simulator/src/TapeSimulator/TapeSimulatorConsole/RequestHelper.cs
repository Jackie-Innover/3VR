using System.Diagnostics;
using System.Threading;

namespace TapeSimulatorConsole
{
    public static class RequestHelper
    {
        private static long _nextSerialNumber;

        public static long NextRequestId()
        {
            //To make _id unique even cross multiple processes
            int processId = Process.GetCurrentProcess().Id;
            long id = (long)processId * 1000000000 + Interlocked.Increment(ref _nextSerialNumber);
            return id;
        }
    }
}