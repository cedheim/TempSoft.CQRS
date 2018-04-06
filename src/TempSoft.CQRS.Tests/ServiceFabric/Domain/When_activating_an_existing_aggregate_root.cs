using System;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Microsoft.ServiceFabric.Actors;
using NUnit.Framework;
using TempSoft.CQRS.ServiceFabric.Domain;
using TempSoft.CQRS.ServiceFabric.Interfaces.Messaging;
using TempSoft.CQRS.Tests.Mocks;

namespace TempSoft.CQRS.Tests.ServiceFabric.Domain
{
    [TestFixture]
    public class When_activating_an_existing_aggregate_root : AggregateRootActorTestBase
    {
        private AggregateRootActor _actor;
        private AThingAggregateRoot _root;
        private ActorId _actorId;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _root = new AThingAggregateRoot();
            _root.Initialize(Data.ActorId);
            await _root.Handle(new DoSomething(12312, "WAAAAAA"), CancellationToken.None);
            _root.Commit();

            A.CallTo(() => AggregateRootRepository.Get(A<Type>.Ignored, A<Guid>.Ignored, A<CancellationToken>.Ignored))
                .Returns(_root);
            
            _actorId = new ActorId(Data.ActorId);
            _actor = ActorService.Activate(_actorId);

            await _actor.Handle(new CommandMessage(typeof(AThingAggregateRoot), new DoSomething(Data.AValue, Data.BValue)), CancellationToken.None);
        }

        [Test]
        public void Should_have_updated_the_root()
        {
            _root.Should().NotBeNull();
            _root.Id.Should().Be(Data.ActorId);
            _root.A.Should().Be(Data.AValue);
            _root.B.Should().Be(Data.BValue);
        }


        [Test]
        public void Should_have_gotten_the_root_from_the_repository()
        {
            A.CallTo(() => AggregateRootRepository.Get(typeof(AThingAggregateRoot), Data.ActorId, A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_have_saved_the_aggregate_to_the_repository()
        {
            A.CallTo(() => AggregateRootRepository.Save(_root, A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        private static class Data
        {
            public static readonly Guid ActorId = Guid.NewGuid();
            public const int AValue = 5;
            public const string BValue = "TEST";
        }
    }
}