using System;
using System.Threading;
using Softing.OPCToolbox;
using Softing.OPCToolbox.Client;

namespace Movares
{
    public class Console
    {
        public static void Main(String[] args)
        {
            MyDaSession session = OpcUtils.CreateSession("http://localhost:8079/OPC/DA");
            MyDaSubscription subscription = session.AddSubscription(1000);
            Configuration config = OpcUtils.LoadConfiguration("config.json");
            MovaresClient opcClient = new MovaresClient(config, subscription, session, 1000);

            // Initialize the client instance
            if (!ResultCode.SUCCEEDED(opcClient.Initialize()))
            {
                throw new Exception("Failed to initialize the client instance");
            }
            
            while (true)
            {
                System.Console.WriteLine("Hey");
                opcClient.Update();
            }
            
            opcClient.Terminate();
        }
    }
}