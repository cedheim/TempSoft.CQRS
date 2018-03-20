using System;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using NCG.NGS.CQRS.ServiceFabric.Interfaces.Domain;
using NCG.NGS.CQRS.ServiceFabric.Interfaces.Messaging;
using NCG.NGS.CQRS.Tests.Mocks;
using NUnit.Framework;

namespace NCG.NGS.CQRS.Tests.ServiceFabric.Commands.ServiceFabricCommandRouter
{
    [TestFixture]
    public class When_initializing_an_aggregate_root
    {
        private IActorProxyFactory _actorProxyFactory;
        private IAggregateRootActor _actor;
        private CQRS.ServiceFabric.Commands.ServiceFabricCommandRouter _router;
        private Guid _actorId;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _actorProxyFactory = A.Fake<IActorProxyFactory>();
            _actor = A.Fake<IAggregateRootActor>();

            A.CallTo(() => _actorProxyFactory.CreateActorProxy<IAggregateRootActor>(A<ActorId>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored))
                .Returns(_actor);

            _router = new CQRS.ServiceFabric.Commands.ServiceFabricCommandRouter(_actorProxyFactory);

            _actorId = await _router.Initialize<AThingAggregateRoot>(CancellationToken.None);
        }

        [Test]
        public void Should_have_created_an_actor_proxy()
        {
            A.CallTo(() => _actorProxyFactory.CreateActorProxy<IAggregateRootActor>(A<ActorId>.That.Matches(a => a.GetGuidId() == _actorId), A<string>.Ignored, A<string>.Ignored, A<string>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_have_called_initialize_on_the_actor()
        {
            A.CallTo(() => _actor.Initialize(A<InitializeMessage>.That.Matches(msg => msg.AggregateRootType == typeof(AThingAggregateRoot)), A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_have_returned_a_proper_id()
        {
            _actorId.Should().NotBeEmpty();
        }
    }
}