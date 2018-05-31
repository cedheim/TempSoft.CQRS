using System.Fabric;

namespace TempSoft.CQRS.ServiceFabric.Tools
{
    public class FabricClientWrapper : IFabricClient
    {
        private readonly FabricClient _client;

        public FabricClientWrapper(FabricClient client)
        {
            _client = client;
        }

        public IQueryClient QueryManager => (QueryClientWrapper) _client.QueryManager;

        public static implicit operator FabricClientWrapper(FabricClient client)
        {
            return new FabricClientWrapper(client);
        }

        public static implicit operator FabricClient(FabricClientWrapper wrapper)
        {
            return wrapper._client;
        }
    }
}