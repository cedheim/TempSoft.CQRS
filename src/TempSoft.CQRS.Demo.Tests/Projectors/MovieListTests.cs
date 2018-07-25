using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using TempSoft.CQRS.Demo.Domain.Movies.Events;
using TempSoft.CQRS.Demo.Projectors.MovieLists;
using TempSoft.CQRS.Demo.Projectors.MovieLists.Models;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.Projectors;

namespace TempSoft.CQRS.Demo.Tests.Projectors
{
    [TestFixture]
    public class MovieListTests
    {
        private IProjectionModelRepository _repository;
        private MovieList _projector;
        private IEventStore _store;

        [SetUp]
        public void SetUp()
        {
            _store = A.Fake<IEventStore>();
            _repository = A.Fake<IProjectionModelRepository>();
            _projector = new MovieList(_repository, _store)
            {
                ProjectorId = Data.ProjectorId1
            };
        }

        [Test]
        public async Task Should_initialize_the_projector_on_initial_call()
        {
            A.CallTo(() => _repository.Get<MovieListState>(A<string>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored)).Returns(default(MovieListState));

            await _projector.Project(new MovieCreated(Data.Movie1OriginalTitle){ AggregateRootId  = Data.AggregateRootId1, Version = 1 }, CancellationToken.None);

            A.CallTo(() => _repository.Get<MovieListState>($"{Data.ProjectorId1}_State", Data.ProjectorId1, A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _store.List(A<EventStoreFilter>.That.Matches(filter => filter.EventTypes.Contains(typeof(MovieCreated))), A<Func<IEvent, CancellationToken, Task>>.That.IsNotNull(), A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _repository.Save(A<MovieListState>.That.IsNotNull(), A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _repository.Save(A<MovieListEntry>.Ignored, A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public async Task Should_not_initialize_if_there_already_is_a_state_stored()
        {
            A.CallTo(() => _repository.Get<MovieListState>(A<string>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored)).Returns(new MovieListState
            {
                Id = $"{Data.ProjectorId1}_State",
                ProjectorId = Data.ProjectorId1
            });

            await _projector.Project(new MovieCreated(Data.Movie1OriginalTitle) { AggregateRootId = Data.AggregateRootId1, Version = 1 }, CancellationToken.None);

            A.CallTo(() => _repository.Get<MovieListState>($"{Data.ProjectorId1}_State", Data.ProjectorId1, A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _store.List(A<EventStoreFilter>.Ignored, A<Func<IEvent, CancellationToken, Task>>.Ignored, A<CancellationToken>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => _repository.Save(A<MovieListState>.Ignored, A<CancellationToken>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => _repository.Save(A<MovieListEntry>.Ignored, A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }


        private static class Data
        {
            public const string ProjectorId1 = nameof(MovieList);
            public const string Movie1OriginalTitle = "Movie1";
            public static readonly Guid AggregateRootId1 = Guid.NewGuid();
        }

    }
}