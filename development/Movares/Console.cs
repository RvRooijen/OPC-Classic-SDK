using System;
using System.Threading;
using Movares.DaConsole;
using Softing.OPCToolbox;
using Softing.OPCToolbox.Client;

namespace Movares
{
    public class Console
    {
        public static void Main(String[] args)
        {
            try
            {
                Configuration config = OpcUtils.LoadConfiguration("config.json");
                MovaresClient opcClient = new MovaresClient(config);
        
                while (true)
                {
                    opcClient.Update();
                    Thread.Sleep(1000);
                }
        
                opcClient.Terminate();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
