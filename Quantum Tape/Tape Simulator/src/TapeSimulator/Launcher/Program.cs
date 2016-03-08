using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Xml.Linq;

namespace Launcher
{
    class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;            
            bool createdNew;
            Mutex mutex = new Mutex(true, Assembly.GetExecutingAssembly().FullName, out createdNew);
            if (!createdNew)
            {
                Console.WriteLine("Another Launcher is running!");
                Console.ReadKey();
                Environment.Exit(0);
            }

            CheckRunningTapeSimulator();

            string baseGuid = GetBaseGuid();
            if (string.IsNullOrEmpty(baseGuid))
            {
                Console.ReadKey();
                Environment.Exit(0);
            }

            LaunchTapeSimulator(baseGuid);

            Console.WriteLine("Launch over.");
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine("Error: unhandled exception." + Environment.NewLine + e.ExceptionObject);
            Console.ReadKey();
            Environment.Exit(0);
        }

        private static void LaunchTapeSimulator(string baseGuid)
        {
            int count;
            while (true)
            {
                Console.WriteLine("How many simulators do you want to launch(1~100)?");
                string countValue = Console.ReadLine();
                if (!int.TryParse(countValue, out count) || count <= 0 || count > 100)
                {
                    Console.WriteLine("Pleas input valid data.");
                }
                else
                {
                    break;
                }
            }

            long clientIndex = 100000000000;
            for (int i = 0; i < count; i++)
            {
                clientIndex++;
                Process.Start("TapeSimulatorConsole.exe", baseGuid + clientIndex);
                Thread.Sleep(1000);
            }
        }

        private static string GetBaseGuid()
        {
            string baseGuid = string.Empty;
            try
            {
                XElement rootElement = XElement.Load("TapeSimulatorConfig.xml");
                baseGuid = rootElement.Element("ClientBaseGuid").Value;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fail to load TapeSimulatorConfig.xml, please check it." + Environment.NewLine + ex);
            }
            return baseGuid;
        }

        private static void CheckRunningTapeSimulator()
        {
            Process[] tapeSimulatorProcesses = Process.GetProcessesByName("TapeSimulatorConsole");
            if (tapeSimulatorProcesses.Any())
            {
                Console.WriteLine("There are some TapeSimulatorConsole.exe still running.");
                Console.WriteLine("Do you want to kill old TapeSimulatorConsole and start new TapeSimulatorConsole(Y/N)?");
                string input = Console.ReadLine();
                if (string.Compare(input, "y", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    foreach (Process process in tapeSimulatorProcesses)
                    {
                        process.Kill();
                    }
                }
                else
                {
                    Environment.Exit(0);
                }
            }
        }
    }
}
