using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.ServiceFabric.Actors;
using NUnit.Framework;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.Mocks;
using TempSoft.CQRS.Projectors;
using TempSoft.CQRS.ServiceFabric.Interfaces.Messaging;
using TempSoft.CQRS.ServiceFabric.Interfaces.Projectors;
using TempSoft.CQRS.ServiceFabric.Tests.Mocks;

namespace TempSoft.CQRS.ServiceFabric.Tests.Events.EventBusServices
{
    [TestFixture]
    public class When_publishing_events_and_there_is_an_exception : EventBusServiceTestBase
    {
        private IEvent[] _events;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            A.CallTo(() => ProjectorActor.Project(A<ProjectorMessage>.Ignored, A<CancellationToken>.Ignored))
                .ThrowsAsync(new Exception());
            A.CallTo(() => Registry.ListDefinitionsByEvent(A<IEvent>.Ignored))
                .Returns(new[] { new ProjectorDefinition(nameof(AThingProjector), nameof(AThingProjector), typeof(AThingProjector), new List<Type>(), new List<string>()) });

            _events = new IEvent[]
            {
                new CreatedAThing(Data.RootId) {Version = 1}
            };

            var messages = _events.Select(e => new EventMessage(e)).ToArray();

            await Service.Publish(messages, CancellationToken.None);

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(TimeSpan.FromMilliseconds(10000));

            try
            {
                await Service.InvokeRunAsync(cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
            }
        }

        [Test]
        public void Should_have_listed_project_definitions_for_event()
        {
            A.CallTo(() => Registry.ListDefinitionsByEvent(_events[0]))
                .MustHaveHappened(Repeated.Exactly.Times(10));
        }

        [Test]
        public void Should_have_projected_the_event()
        {
            A.CallTo(() => ProjectorActor.Project(A<ProjectorMessage>.That.Matches(pm => object.ReferenceEquals(pm.Body, _events[0])), A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Times(10));
        }

        [Test]
        public void Should_have_created_the_actor_proxy()
        {
            A.CallTo(() => ActorProxyFactory.CreateActorProxy<IProjectorActor>(A<Uri>.Ignored, A<ActorId>.Ignored, A<string>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Times(10));
        }

        private static class Data
        {
            public static readonly Guid RootId = Guid.NewGuid();
        }
    }
}