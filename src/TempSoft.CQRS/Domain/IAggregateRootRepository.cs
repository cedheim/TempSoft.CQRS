using System;
using System.Threading;
using System.Threading.Tasks;

namespace TempSoft.CQRS.Domain
{
    public interface IAggregateRootRepository
    {
        Task<IAggregateRoot> Get(Type type, string id, bool createIfItDoesNotExist = true, CancellationToken cancellationToken = default(CancellationToken));

        Task<TAggregate> Get<TAggregate>(string id, bool createIfItDoesNotExist = true, CancellationToken cancellationToken = default(CancellationToken))
            where TAggregate : IAggregateRoot;

        Task Save(IAggregateRoot root, CancellationToken cancellationToken = default(CancellationToken));
    }
}