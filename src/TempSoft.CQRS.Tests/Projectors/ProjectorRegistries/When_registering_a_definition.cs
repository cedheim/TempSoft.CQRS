using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TempSoft.CQRS.Exceptions;
using TempSoft.CQRS.Mocks;
using TempSoft.CQRS.Projectors;

namespace TempSoft.CQRS.Tests.Projectors.ProjectorRegistries
{
    [TestFixture]
    public class When_registering_a_definition
    {
        private ProjectorDefinition _definition1;
        private ProjectorDefinition _definition2;
        private ChangedAValue _event1;
        private ChangedBValue _event2;
        private ChangedAValue _event3;
        private ProjectorRegistry _registry;

        [SetUp]
        public void SetUp()
        {
            _event1 = new ChangedAValue(5) { EventGroup = nameof(AThingAggregateRoot) };
            _event2 = new ChangedBValue("TEST") { EventGroup = nameof(AThingAggregateRoot) };
            _event3 = new ChangedAValue(5) { EventGroup = $"Not{nameof(AThingAggregateRoot)}" };


            _definition1 = new ProjectorDefinition("Definition1", "{AggregateRootId}", typeof(AThingProjector), new[] { typeof(ChangedAValue) }, new[] { nameof(AThingAggregateRoot) });
            _definition2 = new ProjectorDefinition("Definition2", "{AggregateRootId}", typeof(AThingProjector), new[] { typeof(ChangedAValue) }, new string[0] );

            _registry = new ProjectorRegistry();
            _registry.Register(_definition1);
            _registry.Register(_definition2);
        }

        [Test]
        public void Should_be_able_to_match_a_single_project_definition()
        {
            var result = _registry.ListDefinitionsByEvent(_event3).ToArray();

            result.Should().OnlyContain(p => object.ReferenceEquals(p, _definition2));
        }

        [Test]
        public void Should_be_able_to_match_on_multiple_definitions()
        {
            var result = _registry.ListDefinitionsByEvent(_event1).ToArray();

            result.Should().OnlyContain(p => object.ReferenceEquals(p, _definition1) || object.ReferenceEquals(p, _definition2));
        }

        [Test]
        public void Should_be_able_to_match_on_none()
        {
            var result = _registry.ListDefinitionsByEvent(_event2).ToArray();

            result.Should().BeEmpty();
        }

        [Test]
        public void Should_not_be_able_to_register_duplicates()
        {
            _registry.Invoking(r => r.Register(new ProjectorDefinition("Definition1", "{AggregateRootId}", typeof(AThingProjector), new[] { typeof(ChangedAValue) }, new[] { nameof(AThingAggregateRoot) })))
                .Should().Throw<DuplicateProjectorDefinitionException>();
        }

    }
}