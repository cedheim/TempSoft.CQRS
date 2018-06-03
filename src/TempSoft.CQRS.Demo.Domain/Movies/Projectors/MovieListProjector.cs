using System.Threading;
using System.Threading.Tasks;
using TempSoft.CQRS.Demo.Domain.Movies.Events;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.Projectors;

namespace TempSoft.CQRS.Demo.Domain.Movies.Projectors
{
    public class MovieListProjector : IProjector
    {
        private readonly IProjectionModelRepository _repository;
        private readonly IEventStore _eventStore;

        private MovieListProjectionState _state;

        public MovieListProjector(IProjectionModelRepository repository, IEventStore eventStore)
        {
            _repository = repository;
            _eventStore = eventStore;
        }

        public string ProjectorId { get; set; }      

        public async Task Project(IEvent @event, CancellationToken cancellationToken)
        {
            if (_state == null)
            {
                await Initialize(cancellationToken);
            }

            if (@event is MovieInitialized movieInitializedEvent)
            {
                await Handle(movieInitializedEvent, cancellationToken);
            }
        }

        private async Task Handle(MovieInitialized @event, CancellationToken cancellationToken)
        {
            await _repository.Save(new MovieListItemProjection
            {
                ProjectorId = ProjectorId,
                Id = @event.AggregateRootId.ToString(),
                Title = @event.Title,
                PublicId = @event.PublicId
            }, cancellationToken);
        }

        private async Task Initialize(CancellationToken cancellationToken)
        {
            _state = await _repository.Get<MovieListProjectionState>(nameof(MovieListProjectionState), ProjectorId, cancellationToken);
            if (_state == null)
            {
                await _eventStore.List(new EventStoreFilter {EventTypes = new[] {typeof(MovieInitialized)}},
                    async (@event, ct) =>
                    {
                        if (@event is MovieInitialized movieInitializedEvent)
                        {
                            await Handle(movieInitializedEvent, ct);
                        }
                    }, cancellationToken);

                _state = new MovieListProjectionState
                {
                    Id = nameof(MovieListProjectionState),
                    ProjectorId = ProjectorId
                };

                await _repository.Save(_state, cancellationToken);
            }

        }
    }
}