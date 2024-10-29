using System;
using System.Collections.Generic;
using System.Timers;
using System.IO;
using Newtonsoft.Json;
using Softing.OPCToolbox.Client;
using Softing.OPCToolbox;

namespace MovaresClientApp
{
    public class OPCClient
    {
        private DaSession _session;
        private DaSubscription _subscription;
        private Timer _timer;
        private List<DaItem> _items;

        public OPCClient()
        {
            _items = new List<DaItem>();
        }

        public void CreateSession(string url)
        {
            _session = new DaSession(url);
        }

        public void AddSubscription(uint updateRate)
        {
            _subscription = new DaSubscription(updateRate, _session);
            _session.AddDaSubscription(updateRate, _subscription);
        }

        public void LoadConfiguration(string configFilePath)
        {
            var configJson = File.ReadAllText(configFilePath);
            var config = JsonConvert.DeserializeObject<Configuration>(configJson);
            ConfigureItems(config.ItemIds);
        }

        public void ConfigureItems(List<string> itemIds)
        {
            foreach (var itemId in itemIds)
            {
                var item = new DaItem(itemId, _subscription);
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
            foreach (var item in _items)
            {
                item.Read(0, out ValueQT value, out int result);
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
