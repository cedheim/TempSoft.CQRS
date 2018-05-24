using System;
using System.Threading;
using System.Threading.Tasks;
using TempSoft.CQRS.Domain;

namespace TempSoft.CQRS.Commands
{
    public class InMemoryCommandRouter : ICommandRouter
    {
        private readonly IAggregateRootRepository _repository;

        public InMemoryCommandRouter(IAggregateRootRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle<TAggregate>(Guid id, ICommand command, CancellationToken cancellationToken = default(CancellationToken)) where TAggregate : IAggregateRoot
        {
            var root = await _repository.Get<TAggregate>(id, cancellationToken);
            await root.Handle(command, cancellationToken);
        }

        public async Task<TReadModel> GetReadModel<TAggregate, TReadModel>(Guid id, CancellationToken cancellationToken = default(CancellationToken)) where TAggregate : IAggregateRootWithReadModel where TReadModel : IAggregateRootReadModel
        {
            var root = await _repository.Get<TAggregate>(id, cancellationToken);
            return (TReadModel) (root.GetReadModel());
        }
    }
}