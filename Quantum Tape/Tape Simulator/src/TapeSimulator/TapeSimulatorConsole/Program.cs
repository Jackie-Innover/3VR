

using System;


namespace TapeSimulatorConsole
{
    internal class Program
    {
        private static void Main(string[] args)
        {

            if (args.Length <= 0)
            {
                Console.WriteLine("Not pass client GUID, please check it.");
                Environment.Exit(0);
            }

            Guid clientGuid;
            if (!Guid.TryParse(args[0], out clientGuid))
            {
                Console.WriteLine("Client GUID format should be: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX");
                Environment.Exit(0);
            }

            try
            {                
                bool isLoadConfigurationFileCorrectly = TapeSimulatorSetting.Instance.IsLoadConfigurationCorrectly;
                TapeSimulatorSetting.Instance.ClientGuid = clientGuid.ToString("D");
                if (!isLoadConfigurationFileCorrectly)
                {
                    Console.WriteLine("Please check configuration, and restart this application.");
                    Console.ReadKey();
                    return;
                }

                AsyncWebSocketRequests.Instance.Start(TapeSimulatorSetting.Instance.Host,
                    TapeSimulatorSetting.Instance.PortNumber, TapeSimulatorSetting.Instance.UserName,
                    TapeSimulatorSetting.Instance.Password, TapeSimulatorSetting.Instance.ClientGuid,
                    TapeSimulatorSetting.Instance.ClientDisplayName);

                SendManager.SendData();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to start application. Please check and restart the application." + Environment.NewLine + ex);
            }
            Console.ReadKey();
        }
    }
}