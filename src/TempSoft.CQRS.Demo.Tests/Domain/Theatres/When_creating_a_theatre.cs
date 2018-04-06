using System;
using FluentAssertions;
using NUnit.Framework;
using TempSoft.CQRS.Demo.Domain.Theatres.Commands;
using TempSoft.CQRS.Demo.Domain.Theatres.Entities;
using TempSoft.CQRS.Demo.Domain.Theatres.Events;
using TempSoft.CQRS.Domain;

namespace TempSoft.CQRS.Demo.Tests.Domain.Theatres
{
    [TestFixture]
    public class When_creating_a_theatre
    {
        private Theatre _root;
        private InitializeTheatre _command;
        private Commit _commit;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _root = new Theatre();
            _command = new InitializeTheatre(Data.AggregateRootId, Data.Name);
            
            _root.Handle(_command);
            _commit = _root.Commit();
        }

        [Test]
        public void Should_have_generated_an_initialization_event()
        {
            _commit.Events.Should().ContainSingle(e => (e is TheatreInitialized) && ((TheatreInitialized)e).Name == Data.Name && ((TheatreInitialized)e).AggregateRootId == Data.AggregateRootId);
        }

        [Test]
        public void Should_have_contained_the_command()
        {
            _commit.CommandIds.Should().ContainSingle(cid => cid == _command.Id);
        }

        [Test]
        public void Should_have_set_the_aggregate_root_id()
        {
            _root.Id.Should().Be(Data.AggregateRootId);
        }

        [Test]
        public void Should_have_set_the_name()
        {
            _root.Name.Should().Be(Data.Name);
        }

        private static class Data
        {
            public static readonly Guid AggregateRootId = Guid.NewGuid();

            public const string Name = "THEATRE1";
        }
    }
}