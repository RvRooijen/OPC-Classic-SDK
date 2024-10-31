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

        private ValueQT[] _values = [];
        private int[] _results = [];

        public MovaresClient(Configuration config)
        {
            Application.Instance.Initialize();
            _session = new MyDaSession(config.Url);
            _subscription = _session.AddSubscription(config.UpdateRate);
            _items = InitializeDaItems(config, _subscription, _session);
        }

        private MyDaItem[] InitializeDaItems(Configuration config, MyDaSubscription subscription, MyDaSession session)
        {
            var executionOptions = new ExecutionOptions(EnumExecutionType.SYNCHRONOUS, 0);
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

                var connection = session.Connect(true, false, executionOptions);
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
            for (var i = 0; i < _items.Length; i++)
            {
                var item = _items[i];
                var value = _values[i];
                var result = _results[i];
                if (ResultCode.SUCCEEDED(result))
                {
                    System.Console.WriteLine($"Item: {item.Id}, Value: {value.Data}");
                }
                else
                {
                    System.Console.WriteLine($"Failed to read item: {item.Id}");
                }
            }
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
