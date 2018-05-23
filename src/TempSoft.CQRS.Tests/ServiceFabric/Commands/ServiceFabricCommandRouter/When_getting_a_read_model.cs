using System;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using NUnit.Framework;
using TempSoft.CQRS.ServiceFabric.Interfaces.Domain;
using TempSoft.CQRS.ServiceFabric.Interfaces.Messaging;
using TempSoft.CQRS.Tests.Mocks;

namespace TempSoft.CQRS.Tests.ServiceFabric.Commands.ServiceFabricCommandRouter
{
    [TestFixture]
    public class When_getting_a_read_model
    {
        private IActorProxyFactory _actorProxyFactory;
        private IAggregateRootActor _actor;
        private CQRS.ServiceFabric.Commands.ServiceFabricCommandRouter _router;
        private AThingReadModel _readModel;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _actorProxyFactory = A.Fake<IActorProxyFactory>();
            _actor = A.Fake<IAggregateRootActor>();

            A.CallTo(() => _actorProxyFactory.CreateActorProxy<IAggregateRootActor>(A<ActorId>.Ignored,
                    A<string>.Ignored, A<string>.Ignored, A<string>.Ignored))
                .Returns(_actor);
            A.CallTo(() => _actor.GetReadModel(A<GetReadModelMessage>.Ignored, A<CancellationToken>.Ignored))
                .Returns(new ReadModelMessage(new AThingReadModel()));

            _router = new CQRS.ServiceFabric.Commands.ServiceFabricCommandRouter(_actorProxyFactory);
            _readModel =
                await _router.GetReadModel<AThingAggregateRoot, AThingReadModel>(Data.RootId, CancellationToken.None);
        }

        private static class Data
        {
            public static readonly Guid RootId = Guid.NewGuid();
        }

        [Test]
        public void Should_have_created_an_actor_proxy()
        {
            A.CallTo(() => _actorProxyFactory.CreateActorProxy<IAggregateRootActor>(
                    A<ActorId>.That.Matches(a => a.GetGuidId() == Data.RootId), A<string>.Ignored, A<string>.Ignored,
                    A<string>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_have_gotten_the_read_model_on_the_actor()
        {
            A.CallTo(() =>
                    _actor.GetReadModel(
                        A<GetReadModelMessage>.That.Matches(msg =>
                            msg.AggregateRootType == typeof(AThingAggregateRoot)), A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}