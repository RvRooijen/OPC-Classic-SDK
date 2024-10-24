using System;
using Softing.OPCToolbox.Client;

namespace Softing.OPCToolbox.Client
{
    public class ClientApplication
    {
        public MinimalisticOpcUaClient CreateMinimalisticOpcUaClient()
        {
            return new MinimalisticOpcUaClient();
        }
    }
}
