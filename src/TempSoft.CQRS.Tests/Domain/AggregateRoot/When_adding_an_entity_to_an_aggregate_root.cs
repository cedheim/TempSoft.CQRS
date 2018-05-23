using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TempSoft.CQRS.Domain;
using TempSoft.CQRS.Tests.Mocks;

namespace TempSoft.CQRS.Tests.Domain.AggregateRoot
{
    [TestFixture]
    public class When_adding_an_entity_to_an_aggregate_root
    {
        private AThingAggregateRoot _root;
        private Commit _commit;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _root = new AThingAggregateRoot();
            _root.Initialize(Data.RootId);
            await _root.AddStuff(Data.EntityId, Data.StuffMessage, CancellationToken.None);

            _commit = _root.Commit();
        }

        private static class Data
        {
            public const string StuffMessage = "STUFF!!";
            public static readonly Guid RootId = Guid.NewGuid();
            public static readonly Guid EntityId = Guid.NewGuid();
        }

        [Test]
        public void Should_have_added_stuff_to_the_aggregate_root()
        {
            _root.Stuff.Should()
                .ContainSingle(stuff => stuff.Id == Data.EntityId && stuff.Message == Data.StuffMessage);
        }

        [Test]
        public void Should_have_created_an_event()
        {
            _commit.Events.Should().ContainSingle(e =>
                e is AddedStuff && ((AddedStuff) e).EntityId == Data.EntityId &&
                ((AddedStuff) e).Message == Data.StuffMessage);
        }
    }
}