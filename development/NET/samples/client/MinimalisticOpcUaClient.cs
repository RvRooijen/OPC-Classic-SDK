using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;

namespace Softing.OPCToolbox.Client
{
    public class MinimalisticOpcUaClient
    {
        private ApplicationInstance _application;
        private Session _session;

        public MinimalisticOpcUaClient()
        {
            _application = new ApplicationInstance
            {
                ApplicationName = "MinimalisticOpcUaClient",
                ApplicationType = ApplicationType.Client,
                ConfigSectionName = "Opc.Ua.MinimalisticOpcUaClient"
            };
        }

        public async Task Connect(string serverUrl)
        {
            await _application.LoadApplicationConfiguration(false);
            await _application.CheckApplicationInstanceCertificate(false, 0);

            var endpointDescription = CoreClientUtils.SelectEndpoint(serverUrl, false);
            var endpointConfiguration = EndpointConfiguration.Create(_application.ApplicationConfiguration);
            var endpoint = new ConfiguredEndpoint(null, endpointDescription, endpointConfiguration);

            _session = await Session.Create(
                _application.ApplicationConfiguration,
                endpoint,
                false,
                "MinimalisticOpcUaClient",
                60000,
                null,
                null);
        }

        public void SubscribeToTag(string nodeId, MonitoredItemNotificationEventHandler callback)
        {
            var subscription = new Subscription(_session.DefaultSubscription) { PublishingInterval = 1000 };

            var monitoredItem = new MonitoredItem
            {
                StartNodeId = new NodeId(nodeId),
                AttributeId = Attributes.Value
            };

            monitoredItem.Notification += callback;
            subscription.AddItem(monitoredItem);
            _session.AddSubscription(subscription);
            subscription.Create();
        }

        public async Task RequestTagChange(string nodeId, object value)
        {
            var writeValue = new WriteValue
            {
                NodeId = new NodeId(nodeId),
                AttributeId = Attributes.Value,
                Value = new DataValue(new Variant(value))
            };

            var writeValues = new WriteValueCollection { writeValue };
            var response = await _session.WriteAsync(null, writeValues);

            if (response.Results[0] != StatusCodes.Good)
            {
                throw new Exception($"Failed to write value to node {nodeId}: {response.Results[0]}");
            }
        }

        public void Disconnect()
        {
            _session.Close();
            _session.Dispose();
        }
    }
}
