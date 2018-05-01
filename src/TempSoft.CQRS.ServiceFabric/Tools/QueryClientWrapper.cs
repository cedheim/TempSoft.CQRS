using System;
using System.Fabric;
using System.Fabric.Query;
using System.Threading;
using System.Threading.Tasks;

namespace TempSoft.CQRS.ServiceFabric.Tools
{
    public class QueryClientWrapper : IQueryClient
    {
        private readonly FabricClient.QueryClient _client;

        public QueryClientWrapper(FabricClient.QueryClient client)
        {
            _client = client;
        }

        public Task<ServicePartitionList> GetPartitionAsync(Guid partitionId, TimeSpan timeout, CancellationToken cancellationToken)
        {
            return _client.GetPartitionAsync(partitionId, timeout, cancellationToken);
        }

        public Task<ServicePartitionList> GetPartitionAsync(Guid partitionId)
        {
            return _client.GetPartitionAsync(partitionId);
        }


        public static implicit operator QueryClientWrapper(FabricClient.QueryClient client)
        {
            return new QueryClientWrapper(client);
        }

        public static implicit operator FabricClient.QueryClient(QueryClientWrapper wrapper)
        {
            return wrapper._client;
        }
    }
}