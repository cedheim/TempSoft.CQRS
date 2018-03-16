using System;
using System.Threading;
using System.Threading.Tasks;
using NCG.NGS.CQRS.Domain;

namespace NCG.NGS.CQRS.Commands
{
    public interface ICommandBus
    {
        Task<Guid> InitializeAsync<TAggregate>(CancellationToken cancellationToken = default(CancellationToken)) where TAggregate : IAggregateRoot;

        Task HandleAsync<TAggregate>(Guid id, ICommand command, CancellationToken cancellationToken = default(CancellationToken)) where TAggregate : IAggregateRoot;
    }
}