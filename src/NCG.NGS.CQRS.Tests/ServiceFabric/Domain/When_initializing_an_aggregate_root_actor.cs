using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.ServiceFabric.Actors;
using NCG.NGS.CQRS.Events;
using NCG.NGS.CQRS.ServiceFabric.Domain;
using NCG.NGS.CQRS.ServiceFabric.Events;
using NCG.NGS.CQRS.ServiceFabric.Messaging;
using NCG.NGS.CQRS.Tests.Mocks;
using NUnit.Framework;

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
            _actorId = new ActorId(Data.ActorId);
            _actor = ActorService.Activate(_actorId);
            _message = new InitializeMessage(typeof(AThingAggregateRoot));

            await _actor.Initialize(_message, CancellationToken.None);

            _root = _actor.GetRoot<AThingAggregateRoot>();
        }

        [Test]
        public void Should_have_initialized_the_root()
        {
            _root.Should().NotBeNull();
            _root.Id.Should().Be(Data.ActorId);
        }

        [Test]
        public void Should_have_stored_the_events_in_the_state()
        {
            var eventStream = StateManagers[_actorId].AsDictionary()[EventStreamStateName] as EventStream;

            eventStream.Should().ContainSingle(e => e is InitializationEvent);
        }

        private static class Data
        {
            public static readonly Guid ActorId = Guid.NewGuid();
        }
    }
}