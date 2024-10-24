using System;
using System.Threading.Tasks;
using Softing.OPCToolbox.Client;

namespace Softing.OPCToolbox.Client
{
    public class MinimalisticClient
    {
        private MinimalisticOpcUaClient _client;

        public MinimalisticClient()
        {
            _client = new MinimalisticOpcUaClient();
        }

        public async Task Connect(string serverUrl)
        {
            await _client.Connect(serverUrl);
        }

        public void SubscribeToTag(string nodeId, MonitoredItemNotificationEventHandler callback)
        {
            _client.SubscribeToTag(nodeId, callback);
        }

        public async Task RequestTagChange(string nodeId, object value)
        {
            await _client.RequestTagChange(nodeId, value);
        }

        public void Disconnect()
        {
            _client.Disconnect();
        }

        public static async Task Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: MinimalisticClient <serverUrl> <nodeId>");
                return;
            }

            string serverUrl = args[0];
            string nodeId = args[1];

            MinimalisticClient client = new MinimalisticClient();

            try
            {
                await client.Connect(serverUrl);
                Console.WriteLine("Connected to server.");

                client.SubscribeToTag(nodeId, (monitoredItem, value) =>
                {
                    Console.WriteLine($"Tag value changed: {value.Value}");
                });

                Console.WriteLine("Subscribed to tag.");

                // Simulate a tag change request
                await client.RequestTagChange(nodeId, 42);
                Console.WriteLine("Requested tag value change.");

                Console.WriteLine("Press Enter to disconnect...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                client.Disconnect();
                Console.WriteLine("Disconnected from server.");
            }
        }
    }
}
