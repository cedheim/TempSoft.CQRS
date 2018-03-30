using System;
using System.Threading;
using System.Threading.Tasks;
using TempSoft.CQRS.Domain;

namespace TempSoft.CQRS.Commands
{
    public interface ICommandRouter
    {
        Task<Guid> Initialize<TAggregate>(CancellationToken cancellationToken = default(CancellationToken)) where TAggregate : IAggregateRoot;

        Task Handle<TAggregate>(Guid id, ICommand command, CancellationToken cancellationToken = default(CancellationToken)) where TAggregate : IAggregateRoot;
    }
}