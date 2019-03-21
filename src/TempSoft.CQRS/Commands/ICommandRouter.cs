using System;
using System.Threading;
using System.Threading.Tasks;
using TempSoft.CQRS.Domain;

namespace TempSoft.CQRS.Commands
{
    public interface ICommandRouter
    {
        Task Handle<TAggregate>(string id, ICommand command,
            CancellationToken cancellationToken = default(CancellationToken)) where TAggregate : IAggregateRoot;

        Task<TReadModel> GetReadModel<TAggregate, TReadModel>(string id,
            CancellationToken cancellationToken = default(CancellationToken)) where TAggregate : IAggregateRootWithReadModel
            where TReadModel : IAggregateRootReadModel;
    }
}