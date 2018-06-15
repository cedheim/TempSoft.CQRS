using System;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Microsoft.ServiceFabric.Actors;
using NUnit.Framework;
using TempSoft.CQRS.Mocks;
using TempSoft.CQRS.ServiceFabric.Domain;
using TempSoft.CQRS.ServiceFabric.Interfaces.Messaging;

namespace TempSoft.CQRS.ServiceFabric.Tests.Domain
{
    [TestFixture]
    public class When_getting_the_read_model_that_does_not_exist : AggregateRootActorTestBase
    {
        private AggregateRootActor _actor;
        private ActorId _actorId;
        private ReadModelMessage _readModelMessage;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            A.CallTo(() => AggregateRootRepository.Get(A<Type>.Ignored, A<Guid>.Ignored, A<bool>.Ignored, A<CancellationToken>.Ignored))
                .Returns(default(AThingAggregateRoot));

            _actorId = new ActorId(Data.ActorId);
            _actor = ActorService.Activate(_actorId);

            _readModelMessage = await _actor.GetReadModel(new GetReadModelMessage(typeof(AThingAggregateRoot)),
                CancellationToken.None);
        }

        private static class Data
        {
            public const int AValue = 5;
            public const string BValue = "TEST";
            public static readonly Guid ActorId = Guid.NewGuid();
        }

        [Test]
        public void Should_have_gotten_the_root_from_the_repository()
        {
            A.CallTo(() =>
                    AggregateRootRepository.Get(typeof(AThingAggregateRoot), Data.ActorId, false,
                        A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_have_returned_the_correct_read_model()
        {
            var readModel = _readModelMessage.GetReadModel<AThingReadModel>();
            readModel.Should().BeNull();
        }
    }
}