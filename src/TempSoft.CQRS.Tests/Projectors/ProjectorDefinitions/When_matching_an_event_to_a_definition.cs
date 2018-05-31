using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TempSoft.CQRS.Mocks;
using TempSoft.CQRS.Projectors;

namespace TempSoft.CQRS.Tests.Projectors.ProjectorDefinitions
{
    [TestFixture]
    public class When_matching_an_event_to_a_definition
    {

        [Test]
        public void Should_match_on_event_type()
        {
            var @event = new ChangedAValue(5) {EventGroup = nameof(AThingAggregateRoot)};
            var definition = new ProjectorDefinition(nameof(ProjectorDefinition), "{AggregateRootId}", typeof(AThingProjector), new[] { typeof(ChangedAValue) }, new string[0]);

            definition.Matches(@event).Should().BeTrue();
        }

        [Test]
        public void Should_match_on_event_group()
        {
            var @event = new ChangedAValue(5) { EventGroup = nameof(AThingAggregateRoot) };
            var definition = new ProjectorDefinition(nameof(ProjectorDefinition), "{AggregateRootId}", typeof(AThingProjector), new Type[0], new[]{ nameof(AThingAggregateRoot) });

            definition.Matches(@event).Should().BeTrue();
        }

        [Test]
        public void Should_match_on_both_group_and_type()
        {
            var @event1 = new ChangedAValue(5) { EventGroup = nameof(AThingAggregateRoot) };
            var @event2 = new ChangedBValue("TEST") { EventGroup = nameof(AThingAggregateRoot) };
            var @event3 = new ChangedAValue(5) { EventGroup = $"Not{nameof(AThingAggregateRoot)}" };
            var definition = new ProjectorDefinition(nameof(ProjectorDefinition), "{AggregateRootId}", typeof(AThingProjector), new[] { typeof(ChangedAValue) }, new[] { nameof(AThingAggregateRoot) });

            definition.Matches(@event1).Should().BeTrue();
            definition.Matches(@event2).Should().BeFalse();
            definition.Matches(@event3).Should().BeFalse();
        }
    }
}