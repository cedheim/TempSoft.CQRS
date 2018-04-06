using System;
using FluentAssertions;
using NUnit.Framework;
using TempSoft.CQRS.Demo.Domain.Movies.Commands;
using TempSoft.CQRS.Demo.Domain.Movies.Entity;
using TempSoft.CQRS.Demo.Domain.Movies.Events;
using TempSoft.CQRS.Domain;

namespace TempSoft.CQRS.Demo.Tests.Domain.Movies
{
    [TestFixture]
    public class When_adding_a_version_to_a_movie
    {
        private Movie _root;
        private Commit _commit;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _root = new Movie();
            _root.Handle(new InitializeMovie(Data.AggregateRootId, Data.PublicId));
            _root.Handle(new AddMovieVersion(Data.MovieVersionId));
            _commit = _root.Commit();
        }

        [Test]
        public void Should_have_added_the_version()
        {
            _root.Versions.Should().ContainSingle(v => v.Id == Data.MovieVersionId);
        }

        [Test]
        public void Should_have_published_the_event()
        {
            _commit.Events.Should().ContainSingle(e => e is AddedMovieVersion && ((AddedMovieVersion)e).VersionId == Data.MovieVersionId && ((AddedMovieVersion)e).AggregateRootId == Data.AggregateRootId);
        }


        private static class Data
        {
            public static readonly Guid AggregateRootId = Guid.NewGuid();
            public static readonly Guid MovieVersionId = Guid.NewGuid();
            public const string PublicId = "MOVIE1";
        }

    }
}