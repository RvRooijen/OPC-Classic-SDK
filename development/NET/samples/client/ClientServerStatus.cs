using System;
using Softing.OPCToolbox.Client;

namespace Softing.OPCToolbox.Client
{
    public class ClientServerStatus
    {
        public MinimalisticOpcUaClientStatus MinimalisticOpcUaClientStatus { get; private set; }

        public ClientServerStatus()
        {
            MinimalisticOpcUaClientStatus = MinimalisticOpcUaClientStatus.Disconnected;
        }

        public void UpdateStatus(MinimalisticOpcUaClientStatus status)
        {
            MinimalisticOpcUaClientStatus = status;
        }
    }
}
