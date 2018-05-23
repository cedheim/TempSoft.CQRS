using System;
using System.Threading;
using System.Threading.Tasks;
using TempSoft.CQRS.Domain;

namespace TempSoft.CQRS.Commands
{
    public interface ICommandRouter
    {
        Task Handle<TAggregate>(Guid id, ICommand command,
            CancellationToken cancellationToken = default(CancellationToken)) where TAggregate : IAggregateRoot;

        Task<TReadModel> GetReadModel<TAggregate, TReadModel>(Guid id,
            CancellationToken cancellationToken = default(CancellationToken)) where TAggregate : IAggregateRoot
            where TReadModel : IAggregateRootReadModel;
    }
}