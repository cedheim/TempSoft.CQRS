using FluentAssertions;
using NUnit.Framework;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.Tests.Mocks;

namespace TempSoft.CQRS.Tests.Events
{
    [TestFixture]
    public class When_matching_an_event_to_a_filter
    {
        [Test]
        public void Should_be_able_to_match_on_both_event_group_and_event_type()
        {
            var e = new ChangedAValue(5) {EventGroup = nameof(AThingAggregateRoot)};
            var filter = new EventFilter
            {
                EventGroups = new[] {nameof(AThingAggregateRoot)},
                EventTypes = new[] {typeof(ChangedAValue)}
            };

            filter.Match(e).Should().BeTrue();
        }

        [Test]
        public void Should_be_able_to_match_on_event_group()
        {
            var e = new ChangedAValue(5) {EventGroup = nameof(AThingAggregateRoot)};
            var filter = new EventFilter {EventGroups = new[] {nameof(AThingAggregateRoot)}};

            filter.Match(e).Should().BeTrue();
        }

        [Test]
        public void Should_be_able_to_match_on_event_type()
        {
            var e = new ChangedAValue(5) {EventGroup = nameof(AThingAggregateRoot)};
            var filter = new EventFilter {EventTypes = new[] {typeof(ChangedAValue)}};

            filter.Match(e).Should().BeTrue();
        }

        [Test]
        public void Should_be_able_to_match_on_event_type_when_there_are_multiple_event_types()
        {
            var e = new ChangedAValue(5) {EventGroup = nameof(AThingAggregateRoot)};
            var filter = new EventFilter {EventTypes = new[] {typeof(ChangedAValue), typeof(ChangedBValue)}};
            filter.Match(e).Should().BeTrue();
        }

        [Test]
        public void Should_not_match_if_either_event_group_or_type_differs()
        {
            var e1 = new ChangedAValue(5) {EventGroup = nameof(AThingAggregateRoot)};
            var e2 = new ChangedBValue("HELLO") {EventGroup = "BOO"};
            var filter = new EventFilter
            {
                EventGroups = new[] {nameof(AThingAggregateRoot)},
                EventTypes = new[] {typeof(ChangedBValue)}
            };

            filter.Match(e1).Should().BeFalse();
            filter.Match(e2).Should().BeFalse();
        }

        [Test]
        public void Should_return_false_if_the_event_type_does_not_match()
        {
            var e = new ChangedAValue(5) {EventGroup = nameof(AThingAggregateRoot)};
            var filter = new EventFilter {EventTypes = new[] {typeof(ChangedBValue)}};
            filter.Match(e).Should().BeFalse();
        }
    }
}