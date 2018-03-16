using System;
using System.Linq;
using FluentAssertions;
using NCG.NGS.CQRS.Events;
using NCG.NGS.CQRS.Tests.Mocks;
using NUnit.Framework;

namespace NCG.NGS.CQRS.Tests.Domain
{
    [TestFixture]
    public class When_invoking_a_command_twice
    {
        private AThingAggregateRoot _root;
        private IEvent[] _events;
        private Guid[] _processedCommands;
        private DoSomething _command;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _root = new AThingAggregateRoot();
            _root.Initialize(Data.RootId);
            _root.Commit();

            _command = new DoSomething(Data.AValue, Data.BValue);
            _root.Handle(_command);
            _root.Handle(_command);
            _events = _root.Commit().ToArray();

            _processedCommands = _root.ProcessedCommands.ToArray();
        }

        [Test]
        public void Should_have_updated_the_values()
        {
            _root.A.Should().Be(Data.AValue);
            _root.B.Should().Be(Data.BValue);
        }

        [Test]
        public void Should_have_processed_the_command_once()
        {
            _processedCommands.Should().ContainSingle(pc => pc == _command.Id);
        }

        [Test]
        public void Should_have_triggered_events()
        {
            _events.Should().ContainSingle(e => e is ChangedAValue);
            _events.Should().ContainSingle(e => e is ChangedBValue);
        }

        [Test]
        public void Should_have_updated_version()
        {
            _root.Version.Should().Be(3);
        }

        private static class Data
        {
            public static readonly Guid RootId = Guid.NewGuid();
            public const int AValue = 5;
            public const string BValue = "FLEUF";
        }


    }
}