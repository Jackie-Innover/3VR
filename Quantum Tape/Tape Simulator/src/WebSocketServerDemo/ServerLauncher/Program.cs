using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace ServerLauncher
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = 24042;
            for (int i = 0; i < 10; i++)
            {
                Process.Start("WebSocketServerDemo.exe", port++.ToString());
                Thread.Sleep(1000);
            }
        }
    }
}
