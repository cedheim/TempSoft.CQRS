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
    public class When_handling_a_command
    {
        private IActorProxyFactory _actorProxyFactory;
        private IAggregateRootActor _actor;
        private CQRS.ServiceFabric.Commands.ServiceFabricCommandRouter _router;
        private DoSomething _command;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _actorProxyFactory = A.Fake<IActorProxyFactory>();
            _actor = A.Fake<IAggregateRootActor>();

            A.CallTo(() => _actorProxyFactory.CreateActorProxy<IAggregateRootActor>(A<ActorId>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored))
                .Returns(_actor);

            _router = new CQRS.ServiceFabric.Commands.ServiceFabricCommandRouter(_actorProxyFactory);

            _command = new DoSomething(5, "HELLU");
            await _router.Handle<AThingAggregateRoot>(Data.RootId, _command, CancellationToken.None);
        }

        [Test]
        public void Should_have_created_an_actor_proxy()
        {
            A.CallTo(() => _actorProxyFactory.CreateActorProxy<IAggregateRootActor>(A<ActorId>.That.Matches(a => a.GetGuidId() == Data.RootId), A<string>.Ignored, A<string>.Ignored, A<string>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_have_called_handle_on_the_actor()
        {
            A.CallTo(() => _actor.Handle(A<CommandMessage>.That.Matches(msg => msg.AggregateRootType == typeof(AThingAggregateRoot) && msg.Body.Id == _command.Id), A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
        
        private static class Data
        {
            public static readonly Guid RootId = Guid.NewGuid();
        }
    }
}