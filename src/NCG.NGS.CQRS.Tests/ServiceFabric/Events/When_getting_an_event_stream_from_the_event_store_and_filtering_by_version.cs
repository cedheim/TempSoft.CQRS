using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Data;
using NCG.NGS.CQRS.Events;
using NCG.NGS.CQRS.ServiceFabric.Events;
using NCG.NGS.CQRS.Tests.Mocks;
using NUnit.Framework;

namespace NCG.NGS.CQRS.Tests.ServiceFabric.Events
{
    [TestFixture]
    public class When_getting_an_event_stream_from_the_event_store_and_filtering_by_version
    {
        private IActorStateManager _stateManager;
        private ActorEventStore _store;
        private IEvent[] _events;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _stateManager = A.Fake<IActorStateManager>();
            _store = new ActorEventStore(_stateManager);

            A.CallTo(() => _stateManager.TryGetStateAsync<EventStream>(A<string>.That.IsNotNull(), A<CancellationToken>.Ignored))
                .Returns(new ConditionalValue<EventStream>(true, new EventStream(new IEvent[] { new ChangedAValue(5) { Version = 2 }, new InitializationEvent(Data.ActorId) { Version = 1 } })));
            
            _events = (await _store.Get(Data.ActorId, 1, CancellationToken.None)).ToArray();
        }

        [Test]
        public void Should_be_able_to_get_a_stream()
        {
            _events.Should().NotBeNull();
            _events.Should().NotContain(e => e.Version == 1);
            _events.Should().ContainSingle(e => e.Version == 2);
        }

        [Test]
        public void Should_have_tried_to_get_the_state()
        {
            A.CallTo(() => _stateManager.TryGetStateAsync<EventStream>(A<string>.That.IsNotNull(), A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
        
        private static class Data
        {
            public static readonly Guid ActorId = Guid.NewGuid();
        }

    }
}