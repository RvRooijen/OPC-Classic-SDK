using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Opc.Ua;
using Opc.Ua.Server;

namespace Softing.OPCToolbox.Server
{
    public class MinimalisticServer : StandardServer
    {
        private Dictionary<string, DataValue> _tags;

        public MinimalisticServer()
        {
            _tags = new Dictionary<string, DataValue>();
        }

        protected override void OnServerStarting(ApplicationConfiguration configuration)
        {
            base.OnServerStarting(configuration);
            Console.WriteLine("Server is starting...");
        }

        protected override void OnServerStarted(IServerInternal server)
        {
            base.OnServerStarted(server);
            Console.WriteLine("Server started.");
        }

        protected override void OnServerStopping()
        {
            base.OnServerStopping();
            Console.WriteLine("Server is stopping...");
        }

        protected override void OnServerStopped()
        {
            base.OnServerStopped();
            Console.WriteLine("Server stopped.");
        }

        protected override void OnCreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
        {
            base.OnCreateAddressSpace(externalReferences);

            // Create a folder to hold the tags
            var folder = new FolderState(null)
            {
                SymbolicName = "Tags",
                NodeId = new NodeId("Tags"),
                BrowseName = new QualifiedName("Tags"),
                DisplayName = new LocalizedText("Tags"),
                TypeDefinitionId = ObjectTypeIds.FolderType
            };

            // Add the folder to the address space
            AddPredefinedNode(SystemContext, folder);

            // Add some example tags
            AddTag(folder, "Tag1", 0);
            AddTag(folder, "Tag2", 0);
        }

        private void AddTag(FolderState folder, string tagName, object initialValue)
        {
            var tag = new BaseDataVariableState(folder)
            {
                SymbolicName = tagName,
                NodeId = new NodeId(tagName),
                BrowseName = new QualifiedName(tagName),
                DisplayName = new LocalizedText(tagName),
                TypeDefinitionId = VariableTypeIds.BaseDataVariableType,
                DataType = DataTypeIds.Int32,
                ValueRank = ValueRanks.Scalar,
                AccessLevel = AccessLevels.CurrentReadOrWrite,
                UserAccessLevel = AccessLevels.CurrentReadOrWrite,
                Value = initialValue
            };

            folder.AddChild(tag);
            _tags[tagName] = new DataValue(new Variant(initialValue));
        }

        protected override void OnReadValue(
            ISystemContext context,
            NodeState node,
            ref object value,
            ref StatusCode statusCode,
            ref DateTime timestamp)
        {
            if (_tags.TryGetValue(node.SymbolicName, out var dataValue))
            {
                value = dataValue.Value;
                statusCode = dataValue.StatusCode;
                timestamp = dataValue.SourceTimestamp;
            }
            else
            {
                base.OnReadValue(context, node, ref value, ref statusCode, ref timestamp);
            }
        }

        protected override void OnWriteValue(
            ISystemContext context,
            NodeState node,
            ref object value,
            ref StatusCode statusCode,
            ref DateTime timestamp)
        {
            if (_tags.ContainsKey(node.SymbolicName))
            {
                _tags[node.SymbolicName] = new DataValue(new Variant(value));
                statusCode = StatusCodes.Good;
                timestamp = DateTime.UtcNow;
            }
            else
            {
                base.OnWriteValue(context, node, ref value, ref statusCode, ref timestamp);
            }
        }

        public static void Main(string[] args)
        {
            var server = new MinimalisticServer();
            var application = new ApplicationInstance
            {
                ApplicationName = "MinimalisticServer",
                ApplicationType = ApplicationType.Server,
                ConfigSectionName = "Opc.Ua.MinimalisticServer"
            };

            try
            {
                application.Start(server).Wait();
                Console.WriteLine("Press Enter to exit...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                server.Stop();
            }
        }
    }
}
