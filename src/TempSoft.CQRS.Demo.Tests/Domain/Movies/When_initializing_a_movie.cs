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
    public class When_initializing_a_movie
    {
        private Movie _root;
        private Commit _commit;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _root = new Movie();
            _root.Handle(new InitializeMovie(Data.AggregateRootId, Data.PublicId));

            _commit = _root.Commit();
        }

        [Test]
        public void Should_have_set_the_properties()
        {
            _root.Id.Should().Be(Data.AggregateRootId);
            _root.PublicId.Should().Be(Data.PublicId);
        }

        [Test]
        public void Should_have_published_the_event()
        {
            _commit.Events.Should().ContainSingle(e => e is MovieInitialized && ((MovieInitialized)e).PublicId == Data.PublicId && ((MovieInitialized)e).AggregateRootId == Data.AggregateRootId);
        }


        private static class Data
        {
            public static readonly Guid AggregateRootId = Guid.NewGuid();
            public const string PublicId = "MOVIE1";
        }

    }
}