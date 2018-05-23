using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TempSoft.CQRS.Demo.Domain.Movies.Commands;
using TempSoft.CQRS.Demo.Domain.Movies.Entity;
using TempSoft.CQRS.Demo.Domain.Movies.Enums;
using TempSoft.CQRS.Demo.Domain.Movies.Events;
using TempSoft.CQRS.Demo.Domain.Movies.Exceptions;
using TempSoft.CQRS.Demo.Domain.Movies.Models;
using TempSoft.CQRS.Domain;
using Version = TempSoft.CQRS.Demo.Domain.Movies.Entity.Version;

namespace TempSoft.CQRS.Demo.Tests.Domain
{
    [TestFixture]
    public class Movies
    {
        [SetUp]
        public async Task SetUp()
        {
            _root = new Movie();
            await _root.Handle(new InitializeMovie(Data.AggregateRootId, Data.PublicId, Data.Title),
                CancellationToken.None);
            await _root.Handle(new AddMovieVersion(Data.MovieVersionId, Data.VersionName), CancellationToken.None);
            _commit = _root.Commit();

            _version = _root.Versions.FirstOrDefault(v => v.Id == Data.MovieVersionId);
        }

        private Movie _root;
        private Commit _commit;
        private Version _version;


        private static class Data
        {
            public const string VersionName = "VERSION!";
            public const string PublicId = "MOVIE1";
            public const string Title = "TITLE!";
            public static readonly Guid AggregateRootId = Guid.NewGuid();
            public static readonly Guid MovieVersionId = Guid.NewGuid();
        }

        [Test]
        public async Task When_adding_a_movie_property()
        {
            await _root.Handle(new AddMovieProperty(Data.MovieVersionId, MovieProperties.Has3D),
                CancellationToken.None);

            var commit = _root.Commit();

            _version.Has3D.Should().BeTrue();

            commit.Events.Should().ContainSingle(e =>
                e is AddedMovieProperty && ((AddedMovieProperty) e).MovieProperty == MovieProperties.Has3D &&
                ((AddedMovieProperty) e).EntityId == Data.MovieVersionId);
        }

        [Test]
        public void When_adding_a_movie_version()
        {
            _root.Versions.Should().ContainSingle(v => v.Id == Data.MovieVersionId && v.Name == Data.VersionName);
            _commit.Events.Should().ContainSingle(e =>
                e is AddedMovieVersion && ((AddedMovieVersion) e).VersionId == Data.MovieVersionId &&
                ((AddedMovieVersion) e).AggregateRootId == Data.AggregateRootId &&
                ((AddedMovieVersion) e).Name == Data.VersionName);
        }

        [Test]
        public async Task When_adding_two_movie_properties()
        {
            await _root.Handle(new AddMovieProperty(Data.MovieVersionId, MovieProperties.Has3D),
                CancellationToken.None);

            _root.Invoking(r =>
                    r.Handle(new AddMovieProperty(Data.MovieVersionId, MovieProperties.Has3D), CancellationToken.None)
                        .Wait())
                .Should().Throw<MoviePropertyException>();
        }

        [Test]
        public void When_getting_the_read_model()
        {
            var readModel = _root.GetReadModel() as MovieReadModel;
            readModel.Should().BeEquivalentTo(_root);
        }

        [Test]
        public void When_initializing_movie()
        {
            _root.Id.Should().Be(Data.AggregateRootId);
            _root.PublicId.Should().Be(Data.PublicId);
            _root.Title.Should().Be(Data.Title);
            _commit.Events.Should().ContainSingle(e =>
                e is MovieInitialized && ((MovieInitialized) e).PublicId == Data.PublicId &&
                ((MovieInitialized) e).AggregateRootId == Data.AggregateRootId &&
                ((MovieInitialized) e).Title == Data.Title);
        }

        [Test]
        public async Task When_removing_movie_property()
        {
            await _root.Handle(new AddMovieProperty(Data.MovieVersionId, MovieProperties.Has3D),
                CancellationToken.None);
            await _root.Handle(new RemoveMovieProperty(Data.MovieVersionId, MovieProperties.Has3D),
                CancellationToken.None);

            _version.Has3D.Should().BeFalse();

            var commit = _root.Commit();
            commit.Events.Should().ContainSingle(e =>
                e is RemovedMovieProperty && ((RemovedMovieProperty) e).MovieProperty == MovieProperties.Has3D &&
                ((RemovedMovieProperty) e).EntityId == Data.MovieVersionId);
        }

        [Test]
        public void When_removing_movie_property_that_does_not_exist()
        {
            _root.Invoking(r =>
                    r.Handle(new RemoveMovieProperty(Data.MovieVersionId, MovieProperties.Has3D),
                        CancellationToken.None).Wait())
                .Should().Throw<MoviePropertyException>();
        }
    }
}