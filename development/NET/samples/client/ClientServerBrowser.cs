using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Softing.OPCToolbox.Client
{
    public class ClientServerBrowser
    {
        public async Task<List<string>> BrowseMinimalisticOpcUaClient(MinimalisticOpcUaClient client)
        {
            var serverUrls = new List<string>();

            try
            {
                var endpoints = await client.GetEndpoints();
                foreach (var endpoint in endpoints)
                {
                    serverUrls.Add(endpoint.EndpointUrl);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error browsing OPC UA servers: {ex.Message}");
            }

            return serverUrls;
        }
    }
}
