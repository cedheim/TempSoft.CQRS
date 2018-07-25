using System;
using System.Threading;
using System.Threading.Tasks;
using TempSoft.CQRS.Demo.Domain.Movies.Events;
using TempSoft.CQRS.Demo.Projectors.MovieLists.Models;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.Projectors;

namespace TempSoft.CQRS.Demo.Projectors.MovieLists
{
    public class MovieList : ProjectorBase<MovieList>
    {
        private readonly IProjectionModelRepository _repository;
        private readonly IEventStore _store;
        private MovieListState _state;

        public MovieList(IProjectionModelRepository repository, IEventStore store)
        {
            _state = default(MovieListState);
            _repository = repository;
            _store = store;
        }
        
        [Projector(typeof(MovieCreated))]
        public async Task MovieCreated(MovieCreated movieCreated, CancellationToken cancellationToken)
        {
            await Initialize(cancellationToken);

            await ProjectEvent(movieCreated, cancellationToken);
        }

        private async Task Initialize(CancellationToken cancellationToken)
        {
            var stateId = $"{ProjectorId}_State";

            if (!object.ReferenceEquals(_state, default(MovieListState)))
            {
                return;
            }

            _state = await _repository.Get<MovieListState>(stateId, ProjectorId, cancellationToken);

            if (object.ReferenceEquals(_state, default(MovieListState)))
            {
                await _store.List(new EventStoreFilter
                {
                    EventTypes = new[] {typeof(MovieCreated)}
                }, ProjectEvent, cancellationToken);

                _state = new MovieListState
                {
                    Id = stateId,
                    ProjectorId = ProjectorId
                };
                await _repository.Save(_state, cancellationToken);
            }
        }

        private async Task ProjectEvent(IEvent @event, CancellationToken cancellationToken)
        {
            if (@event is MovieCreated movieCreated)
            {
                var entryId = $"MovieList_{movieCreated.AggregateRootId}";
                var entry = new MovieListEntry
                {
                    Id = entryId,
                    ProjectorId = ProjectorId,
                    AggregateRootId = movieCreated.AggregateRootId,
                    OriginalTitle = movieCreated.OriginalTitle,
                    Timestamp = movieCreated.Timestamp
                };

                await _repository.Save(entry, cancellationToken);
            }
        }
    }
}