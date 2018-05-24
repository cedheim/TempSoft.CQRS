using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TempSoft.CQRS.Exceptions;
using TempSoft.CQRS.Projectors;
using TempSoft.CQRS.Mocks;

namespace TempSoft.CQRS.Tests.Projectors.ProjectorDefinitions
{
    [TestFixture]
    public class When_generating_an_identifier_for_an_event
    {
        [Test]
        public void Should_be_able_to_generate_an_identifier_that_is_a_constant()
        {
            var @event = new ChangedAValue(5);
            var definition = new ProjectorDefinition(nameof(ProjectorDefinition), "Constant", typeof(AThingProjector), Enumerable.Empty<Type>(), Enumerable.Empty<string>());

            definition.GenerateIdentifierFor(@event).Should().Be("Constant");
        }

        [Test]
        public void Should_be_able_to_generate_an_identifier_based_on_a_field()
        {
            var @event = new ChangedAValue(5);
            var definition = new ProjectorDefinition(nameof(ProjectorDefinition), "{A}", typeof(AThingProjector), Enumerable.Empty<Type>(), Enumerable.Empty<string>());

            definition.GenerateIdentifierFor(@event).Should().Be("5");
        }


        [Test]
        public void Should_be_able_to_generate_an_identifier_based_on_inherited_field()
        {
            var rootId = Guid.NewGuid();
            var @event = new ChangedAValue(5) { AggregateRootId = rootId };
            var definition = new ProjectorDefinition(nameof(ProjectorDefinition), "{AggregateRootId}", typeof(AThingProjector), Enumerable.Empty<Type>(), Enumerable.Empty<string>());

            definition.GenerateIdentifierFor(@event).Should().Be(rootId.ToString());
        }

        [Test]
        public void Should_be_able_to_generate_an_identifier_even_if_a_field_does_not_exist()
        {
            var rootId = Guid.NewGuid();
            var @event = new ChangedAValue(5) { AggregateRootId = rootId };
            var definition = new ProjectorDefinition(nameof(ProjectorDefinition), "{MissingField}", typeof(AThingProjector), Enumerable.Empty<Type>(), Enumerable.Empty<string>());

            definition.GenerateIdentifierFor(@event).Should().Be(string.Empty);
        }

        [Test]
        public void Should_not_be_able_to_parse_an_empty_identifier()
        {
            try
            {
                new ProjectorDefinition(nameof(ProjectorDefinition), string.Empty, typeof(AThingProjector), Enumerable.Empty<Type>(), Enumerable.Empty<string>());

                Assert.Fail();
            }
            catch (ProjectionIdentifierException e) { }
        }

        [Test]
        public void Should_not_be_able_to_parse_an_illegal_identifier()
        {
            try
            {
                new ProjectorDefinition(nameof(ProjectorDefinition), "Illegal{", typeof(AThingProjector), Enumerable.Empty<Type>(), Enumerable.Empty<string>());

                Assert.Fail();
            }
            catch (ProjectionIdentifierException e) { }
        }

        [Test]
        public void Should_be_able_to_generate_complex_identifiers()
        {
            var rootId = Guid.NewGuid();
            var @event = new ChangedAValue(5) { AggregateRootId = rootId };
            var definition = new ProjectorDefinition(nameof(ProjectorDefinition), "ChangedAValue_{AggregateRootId}_{A}_Projector", typeof(AThingProjector), Enumerable.Empty<Type>(), Enumerable.Empty<string>());

            definition.GenerateIdentifierFor(@event).Should().Be($"ChangedAValue_{rootId}_{@event.A}_Projector");
        }
    }
}