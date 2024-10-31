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
                var opcClient = new OpcClient();
                opcClient.Initialize();
                const string url = "http://localhost:8079/OPC/DA";
                var objs = opcClient.InitializeDaObjects(url);
                System.Console.WriteLine(objs);
                
                while (true)
                {
                    opcClient.Update();
                    Thread.Sleep(1000);
                }
            }
            
            catch (Exception e)
            {
                System.Console.WriteLine($"An error occurred: {e.Message}");
            }
        }
        
        public static void Test(String[] args)
        {
            try
            {
                int connectResult = (int)EnumResultCode.E_FAIL;
                MyDaSession session = OpcUtils.CreateSession("http://localhost:8079/OPC/DA");
                // MyDaSubscription subscription = session.AddSubscription(1000);
                
                if (!session.Valid)
                {
                    return;
                }
                
                MyDaSubscription subscription = new MyDaSubscription(1000, session);
                Configuration config = OpcUtils.LoadConfiguration("config.json");
                MovaresClient opcClient = new MovaresClient(config, subscription, session, 1000);
                
                // Initialize the client instance
                if (!ResultCode.SUCCEEDED(opcClient.Initialize()))
                {
                    throw new Exception("Failed to initialize the client instance");
                }
        
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
