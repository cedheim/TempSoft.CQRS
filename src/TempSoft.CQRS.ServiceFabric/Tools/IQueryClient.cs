using System;
using System.Fabric.Query;
using System.Threading;
using System.Threading.Tasks;

namespace TempSoft.CQRS.ServiceFabric.Tools
{
    public interface IQueryClient
    {
        Task<ServicePartitionList> GetPartitionAsync(Guid partitionId, TimeSpan timeout,
            CancellationToken cancellationToken);

        Task<ServicePartitionList> GetPartitionAsync(Guid partitionId);
    }
}