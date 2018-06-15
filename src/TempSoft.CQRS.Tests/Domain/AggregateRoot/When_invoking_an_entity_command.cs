using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TempSoft.CQRS.Domain;
using TempSoft.CQRS.Mocks;

namespace TempSoft.CQRS.Tests.Domain.AggregateRoot
{
    [TestFixture]
    public class When_invoking_an_entity_command
    {
        private AThingAggregateRoot _root;
        private Commit _commit;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _root = new AThingAggregateRoot() {Id = Data.RootId};
            _root.Initialize();
            await _root.AddStuff(Data.EntityId, Data.StuffMessage, CancellationToken.None);
            await _root.Handle(new SetStuffMessage(Data.EntityId, Data.ChangedStuffMessage), CancellationToken.None);

            _commit = _root.Commit();
        }

        private static class Data
        {
            public const string StuffMessage = "STUFF!!";
            public const string ChangedStuffMessage = "MOAR STUFF!!!!";
            public static readonly Guid RootId = Guid.NewGuid();
            public static readonly string EntityId = Guid.NewGuid().ToString();
        }

        [Test]
        public void Should_have_changed_the_stuff_message()
        {
            _root.Stuff.Should()
                .ContainSingle(stuff => stuff.Id == Data.EntityId && stuff.Message == Data.ChangedStuffMessage);
        }

        [Test]
        public void Should_have_created_an_event()
        {
            _commit.Events.Should().ContainSingle(e =>
                e is StuffMessageSet && ((StuffMessageSet) e).EntityId == Data.EntityId &&
                ((StuffMessageSet) e).Message == Data.ChangedStuffMessage);
        }
    }
}