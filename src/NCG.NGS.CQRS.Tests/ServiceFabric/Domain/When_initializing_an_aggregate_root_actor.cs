using System;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Microsoft.ServiceFabric.Actors;
using NCG.NGS.CQRS.Events;
using NCG.NGS.CQRS.ServiceFabric.Domain;
using NCG.NGS.CQRS.ServiceFabric.Events;
using NCG.NGS.CQRS.ServiceFabric.Interfaces.Messaging;
using NCG.NGS.CQRS.Tests.Mocks;
using NUnit.Framework;
using ServiceFabric.Mocks;

namespace NCG.NGS.CQRS.Tests.ServiceFabric.Domain
{
    [TestFixture]
    public class When_initializing_an_aggregate_root_actor : AggregateRootActorTestBase
    {
        private AggregateRootActor _actor;
        private InitializeMessage _message;
        private AThingAggregateRoot _root;
        private ActorId _actorId;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _root = new AThingAggregateRoot();
            A.CallTo(() => Repository.Get(A<Type>.Ignored, A<Guid>.Ignored, A<CancellationToken>.Ignored))
                .Returns(_root);

            _actorId = new ActorId(Data.ActorId);
            _actor = ActorService.Activate(_actorId);

            _message = new InitializeMessage(typeof(AThingAggregateRoot));

            await _actor.Initialize(_message, CancellationToken.None);
        }

        [Test]
        public void Should_have_initialized_the_root()
        {
            _root.Should().NotBeNull();
            _root.Id.Should().Be(Data.ActorId);
        }

        [Test]
        public void Should_have_gotten_the_root_from_the_repository()
        {
            A.CallTo(() => Repository.Get(typeof(AThingAggregateRoot), Data.ActorId, A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_have_saved_the_aggregate_to_the_repository()
        {
            A.CallTo(() => Repository.Save(_root, A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        private static class Data
        {
            public static readonly Guid ActorId = Guid.NewGuid();
        }
    }
}