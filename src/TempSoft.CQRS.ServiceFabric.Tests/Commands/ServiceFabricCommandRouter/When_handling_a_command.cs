using System;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using NUnit.Framework;
using TempSoft.CQRS.Mocks;
using TempSoft.CQRS.ServiceFabric.Interfaces.Domain;
using TempSoft.CQRS.ServiceFabric.Interfaces.Messaging;
using TempSoft.CQRS.ServiceFabric.Tools;

namespace TempSoft.CQRS.ServiceFabric.Tests.Commands.ServiceFabricCommandRouter
{
    [TestFixture]
    public class When_handling_a_command
    {
        private IActorProxyFactory _actorProxyFactory;
        private IAggregateRootActor _actor;
        private CQRS.ServiceFabric.Commands.ServiceFabricCommandRouter _router;
        private DoSomething _command;
        private IUriHelper _uriHelper;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _uriHelper = A.Fake<IUriHelper>();
            _actorProxyFactory = A.Fake<IActorProxyFactory>();
            _actor = A.Fake<IAggregateRootActor>();

            A.CallTo(() => _uriHelper.GetUriFor<IAggregateRootActor>())
                .Returns(new Uri("fabric:/application/actorservice"));
            
            A.CallTo(() => _actorProxyFactory.CreateActorProxy<IAggregateRootActor>(A<Uri>.Ignored, A<ActorId>.Ignored, A<string>.Ignored))
                .Returns(_actor);

            _router = new CQRS.ServiceFabric.Commands.ServiceFabricCommandRouter(_uriHelper, _actorProxyFactory);

            _command = new DoSomething(5, "HELLU");
            await _router.Handle<AThingAggregateRoot>(Data.RootId, _command, CancellationToken.None);
        }

        private static class Data
        {
            public static readonly string RootId = Guid.NewGuid().ToString();
        }

        [Test]
        public void Should_have_called_handle_on_the_actor()
        {
            A.CallTo(() =>
                    _actor.Handle(
                        A<CommandMessage>.That.Matches(msg =>
                            msg.AggregateRootType == typeof(AThingAggregateRoot) && msg.Body.Id == _command.Id),
                        A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_have_created_an_actor_proxy()
        {
            A.CallTo(() => _actorProxyFactory.CreateActorProxy<IAggregateRootActor>(A<Uri>.That.IsNotNull(),
                    A<ActorId>.That.Matches(a => a.GetStringId() == Data.RootId), A<string>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}