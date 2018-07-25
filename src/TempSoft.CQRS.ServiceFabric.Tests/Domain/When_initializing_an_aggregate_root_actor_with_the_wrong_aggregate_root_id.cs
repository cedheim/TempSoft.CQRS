using System;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Microsoft.ServiceFabric.Actors;
using NUnit.Framework;
using TempSoft.CQRS.Mocks;
using TempSoft.CQRS.ServiceFabric.Domain;
using TempSoft.CQRS.ServiceFabric.Exceptions;
using TempSoft.CQRS.ServiceFabric.Interfaces.Messaging;

namespace TempSoft.CQRS.ServiceFabric.Tests.Domain
{
    [TestFixture]
    public class When_initializing_an_aggregate_root_actor_with_the_wrong_aggregate_root_id : AggregateRootActorTestBase
    {
        private AggregateRootActor _actor;
        private CommandMessage _message;
        private AThingAggregateRoot _root;
        private ActorId _actorId;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _root = new AThingAggregateRoot();
            A.CallTo(() => AggregateRootRepository.Get(A<Type>.Ignored, A<Guid>.Ignored, A<bool>.Ignored, A<CancellationToken>.Ignored))
                .Returns(_root);

            _actorId = new ActorId(Data.ActorId);
            _actor = ActorService.Activate(_actorId);

            _message = new CommandMessage(typeof(AThingAggregateRoot), new InitializeAThing(Guid.NewGuid()));

            _actor.Invoking(a => a.Handle(_message, CancellationToken.None).Wait()).Should()
                .Throw<AggregateRootHasWrongIdException>();
        }

        private static class Data
        {
            public static readonly Guid ActorId = Guid.NewGuid();
        }

        [Test]
        public void Should_have_gotten_the_root_from_the_repository()
        {
            A.CallTo(() =>
                    AggregateRootRepository.Get(typeof(AThingAggregateRoot), Data.ActorId, true,
                        A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_not_have_saved_the_aggregate_to_the_repository()
        {
            A.CallTo(() => AggregateRootRepository.Save(A<AThingAggregateRoot>.Ignored, A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Never);
        }
    }
}