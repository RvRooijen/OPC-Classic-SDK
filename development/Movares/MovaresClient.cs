using System.Timers;
using Newtonsoft.Json;
using Softing.OPCToolbox;
using Softing.OPCToolbox.Client;
using Timer = System.Timers.Timer;

namespace Movares
{
    public class MovaresClient
    {
        private MyDaItem[] _items;
        private MyDaSubscription _subscription;
        private MyDaSession _session;
        
        private ValueQT[] _values;
        private int[] _results;

        public MovaresClient(Configuration config, MyDaSubscription subscription, MyDaSession session, int interval)
        {
            _session = session;
            _subscription = subscription;
            _items = InitializeDaItems(config, subscription, session);
        }
        
        public int Initialize()
        {
            int result = (int)EnumResultCode.S_OK;
            Application.Instance.VersionOtb = 447;

            // Log message to indicate the start of initialization
            System.Console.WriteLine("Starting initialization...");

            //	proceed with the OPC Toolkit core initialization
            result = Application.Instance.Initialize();

            if (ResultCode.SUCCEEDED(result))
            {
                //	enable toolkit internal initialization
                Application.Instance.EnableTracing(
                    EnumTraceGroup.ALL,
                    EnumTraceGroup.ALL,
                    EnumTraceGroup.ALL,
                    EnumTraceGroup.ALL,
                    "Trace.txt",
                    1000000,
                    0);
            }   //	end if
            
            return result;

        }   //	end Initialize

        private MyDaItem[] InitializeDaItems(Configuration config, MyDaSubscription subscription, MyDaSession session)
        {
            var items = new MyDaItem[config.ItemIds.Count];
            try
            {
                for (var i = 0; i < config.ItemIds.Count; i++)
                {
                    var item = config.ItemIds[i];
                    var daItem = new MyDaItem(item, subscription);
                    if (!daItem.Valid)
                    {
                        throw new Exception($"Failed to create item: {item}");
                    }
                    items[i] = daItem;
                }

                var connection = session.Connect(true, false, new ExecutionOptions(EnumExecutionType.ASYNCHRONOUS, 0));
                if (!ResultCode.SUCCEEDED(connection))
                {
                    throw new Exception("Failed to connect to the server");
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
            }

            return items;
        }

        public void Update()
        {
            _subscription.Read(100, _items, out _values, out _results, new ExecutionOptions());
        }

        public void Terminate()
        {
            // Log message to indicate the start of termination
            System.Console.WriteLine("Starting termination...");

            _subscription.Disconnect(new ExecutionOptions());
            _session.Disconnect(new ExecutionOptions());
            
            _session.RemoveDaSubscription(_subscription);
            Application.Instance.RemoveDaSession(_session);
            
            Application.Instance.Terminate();
        }
    }
}
