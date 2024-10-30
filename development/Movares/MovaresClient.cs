using System;
using System.Collections.Generic;
using System.Timers;
using System.IO;
using Movares;
using Newtonsoft.Json;
using Softing.OPCToolbox.Client;
using Softing.OPCToolbox;
using Timer = System.Timers.Timer;

namespace MovaresClientApp
{
    public class OpcClient
    {
        private MyDaSession _session;
        private MyDaSubscription _subscription;
        private Timer _timer;
        private List<MyDaItem> _items;

        public OpcClient()
        {
            _items = [];
            SaveConfiguration("config.json");
        }

        public void CreateSession(string url)
        {
            _session = new MyDaSession(url);
        }

        public void AddSubscription(uint updateRate)
        {
            _subscription = new MyDaSubscription(updateRate, _session);
        }

        public void LoadConfiguration(string configFilePath)
        {
            var configJson = File.ReadAllText(configFilePath);
            var config = JsonConvert.DeserializeObject<Configuration>(configJson);
            ConfigureItems(config.ItemIds);
            _session.Connect(true, false, new ExecutionOptions(EnumExecutionType.ASYNCHRONOUS, 0));
        }
        
        public void SaveConfiguration(string configFilePath)
        {
            var config = new Configuration
            {
                ItemIds = ["maths.sin", "time.local.second"]
            };
            var configJson = JsonConvert.SerializeObject(config);
            File.WriteAllText(configFilePath, configJson);
        }

        private void ConfigureItems(List<string> itemIds)
        {
            foreach (var itemId in itemIds)
            {
                var item = new MyDaItem(itemId, _subscription);
                _items.Add(item);
            }
        }

        public void StartUpdating(int interval)
        {
            _timer = new Timer(interval);
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            foreach (DaItem item in _items)
            {
                item.Read(100, out ValueQT value, out int result, new ExecutionOptions(EnumExecutionType.ASYNCHRONOUS, 0));
                if (ResultCode.SUCCEEDED(result))
                {
                    Console.WriteLine($"Item: {item.Id}, Value: {value.Data}, Quality: {value.Quality}, Timestamp: {value.TimeStamp}");
                }
                else
                {
                    Console.WriteLine($"Failed to read item: {item.Id}, Result: {result}");
                }
            }
        }
    }

    public class Configuration
    {
        public List<string> ItemIds { get; set; }
    }
}
