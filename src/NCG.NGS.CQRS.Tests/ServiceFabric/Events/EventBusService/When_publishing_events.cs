using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.ServiceFabric.Actors;
using NCG.NGS.CQRS.Common.Extensions;
using NCG.NGS.CQRS.Events;
using NCG.NGS.CQRS.Queries;
using NCG.NGS.CQRS.ServiceFabric.Interfaces.Messaging;
using NCG.NGS.CQRS.ServiceFabric.Interfaces.Queries;
using NCG.NGS.CQRS.Tests.Mocks;
using NUnit.Framework;

namespace NCG.NGS.CQRS.Tests.ServiceFabric.Events.EventBusService
{
    [TestFixture]
    public class When_publishing_events : EventBusServiceTestBase
    {
        private IEvent[] _events;
        private IQueryBuilder _builder;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _builder = new AThingQueryBuilder(A.Fake<IQueryModelRepository>());

            Registry.Register(_builder);

            A.CallTo(() => ActorProxyFactory.CreateActorProxy<IQueryBuilderActor>(A<ActorId>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored))
                .Returns(Actor);

            _events = new IEvent[]
            {
                new CreatedAThing(Data.RootId) {Version = 1},
                new ChangedAValue(5) {AggregateRootId = Data.RootId, Version = 2}
            };
            
            var messages = _events.Select(e => new EventMessage(e)).ToArray();

            await Service.Publish(messages, CancellationToken.None);
            
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(TimeSpan.FromMilliseconds(500));

            try
            {
                await Service.InvokeRunAsync(cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
            }
        }

        [Test]
        public void Should_have_tried_to_create_an_actor_proxy()
        {
            A.CallTo(() => ActorProxyFactory.CreateActorProxy<IQueryBuilderActor>(A<ActorId>.That.Matches(aId => aId.GetStringId() == _builder.GetType().ToFriendlyName()), A<string>.Ignored, A<string>.Ignored, A<string>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Times(_events.Length));
        }

        [Test]
        public void Should_have_called_the_query_builder_actor()
        {
            foreach (var @event in _events)
            {
                A.CallTo(() => Actor.Apply(A<EventMessage>.That.Matches(em => em.Body.Id == @event.Id), A<CancellationToken>.Ignored))
                    .MustHaveHappened(Repeated.Exactly.Once);
            }
        }

        private static class Data
        {
            public static readonly Guid RootId = Guid.NewGuid();
        }
    }
}