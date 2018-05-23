using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.Exceptions;
using TempSoft.CQRS.Tests.Mocks;

namespace TempSoft.CQRS.Tests.Events
{
    [TestFixture]
    public class When_registering_an_event_stream
    {
        private static class Data
        {
            public const string DefinitionName1 = "DEFINITION1";
            public const string DefinitionName2 = "DEFINITION2";
        }


        [Test]
        public void Should_be_able_to_get_event_stream_by_name()
        {
            var definitions = new[]
            {
                new EventStreamDefinition(Data.DefinitionName1,
                    new EventFilter
                    {
                        EventGroups = new[] {nameof(AThingAggregateRoot)},
                        EventTypes = new[] {typeof(ChangedAValue)}
                    }),
                new EventStreamDefinition(Data.DefinitionName2,
                    new EventFilter
                    {
                        EventGroups = new[] {"ANOTHER EVENT GROUP"},
                        EventTypes = new[] {typeof(ChangedAValue)}
                    })
            };

            var registry = new EventStreamRegistry(definitions);

            registry.GetEventStreamByName(definitions[0].Name).Should().BeSameAs(definitions[0]);
            registry.GetEventStreamByName(definitions[1].Name).Should().BeSameAs(definitions[1]);
        }

        [Test]
        public void Should_be_able_to_register_an_event_stream()
        {
            var registry = new EventStreamRegistry();
            var definition = new EventStreamDefinition(Data.DefinitionName1,
                new EventFilter
                {
                    EventGroups = new[] {nameof(AThingAggregateRoot)},
                    EventTypes = new[] {typeof(ChangedAValue)}
                });

            registry.RegisterEventStream(definition);

            var e = new ChangedAValue(5) {EventGroup = nameof(AThingAggregateRoot)};
            var definitions = registry.GetEventStreamsByEvent(e).ToArray();

            definitions.Should().ContainSingle(d => ReferenceEquals(definition, d));
        }

        [Test]
        public void Should_be_able_to_register_multiple_definitions_with_the_same_filter()
        {
            var definitions = new[]
            {
                new EventStreamDefinition(Data.DefinitionName1,
                    new EventFilter
                    {
                        EventGroups = new[] {nameof(AThingAggregateRoot)},
                        EventTypes = new[] {typeof(ChangedAValue)}
                    }),
                new EventStreamDefinition(Data.DefinitionName2,
                    new EventFilter
                    {
                        EventGroups = new[] {nameof(AThingAggregateRoot)},
                        EventTypes = new[] {typeof(ChangedAValue)}
                    })
            };

            var registry = new EventStreamRegistry(definitions);

            var e = new ChangedAValue(5) {EventGroup = nameof(AThingAggregateRoot)};
            var result = registry.GetEventStreamsByEvent(e).ToArray();

            result.Should().BeEquivalentTo(definitions);
        }

        [Test]
        public void Should_do_correct_matching_when_there_are_multiple_definitions()
        {
            var definitions = new[]
            {
                new EventStreamDefinition(Data.DefinitionName1,
                    new EventFilter
                    {
                        EventGroups = new[] {nameof(AThingAggregateRoot)},
                        EventTypes = new[] {typeof(ChangedAValue)}
                    }),
                new EventStreamDefinition(Data.DefinitionName2,
                    new EventFilter
                    {
                        EventGroups = new[] {"ANOTHER EVENT GROUP"},
                        EventTypes = new[] {typeof(ChangedAValue)}
                    })
            };

            var registry = new EventStreamRegistry(definitions);


            var e = new ChangedAValue(5) {EventGroup = nameof(AThingAggregateRoot)};
            var result = registry.GetEventStreamsByEvent(e).ToArray();

            result.Should().HaveCount(1);
            result.Should().ContainSingle(d => ReferenceEquals(definitions[0], d));
        }

        [Test]
        public void Should_not_be_able_to_register_duplicate_definitions()
        {
            var registry = new EventStreamRegistry();
            var definition1 = new EventStreamDefinition(Data.DefinitionName1,
                new EventFilter
                {
                    EventGroups = new[] {nameof(AThingAggregateRoot)},
                    EventTypes = new[] {typeof(ChangedAValue)}
                });
            var definition2 = new EventStreamDefinition(Data.DefinitionName1,
                new EventFilter
                {
                    EventGroups = new[] {nameof(AThingAggregateRoot)},
                    EventTypes = new[] {typeof(ChangedAValue)}
                });

            registry.RegisterEventStream(definition1);

            registry.Invoking(r => r.RegisterEventStream(definition2)).Should()
                .Throw<DuplicateEventStreamDefinitionException>();
        }
    }
}