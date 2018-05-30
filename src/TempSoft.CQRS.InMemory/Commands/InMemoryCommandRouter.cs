using System;
using System.Threading;
using System.Threading.Tasks;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Domain;

namespace TempSoft.CQRS.InMemory.Commands
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
            await _repository.Save(root, cancellationToken);
        }

        public async Task<TReadModel> GetReadModel<TAggregate, TReadModel>(Guid id, CancellationToken cancellationToken = default(CancellationToken)) where TAggregate : IAggregateRootWithReadModel where TReadModel : IAggregateRootReadModel
        {
            var root = await _repository.Get<TAggregate>(id, cancellationToken);
            return (TReadModel) (root.GetReadModel());
        }
    }
}