using System;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.ServiceFabric.Actors.Runtime;
using NCG.NGS.CQRS.Events;
using NCG.NGS.CQRS.ServiceFabric.Events;
using NCG.NGS.CQRS.Tests.Mocks;
using NUnit.Framework;

namespace NCG.NGS.CQRS.Tests.ServiceFabric.Events.ActorEventStore
{
    [TestFixture]
    public class When_saveing_an_event_stream
    {
        private IActorStateManager _stateManager;
        private CQRS.ServiceFabric.Events.ActorEventStore _store;
        private IEvent[] _events;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _stateManager = A.Fake<IActorStateManager>();
            _store = new CQRS.ServiceFabric.Events.ActorEventStore(_stateManager);

            _events = new IEvent[] {new ChangedAValue(5) {Version = 2}, new CreatedAThing(Data.ActorId) {Version = 1}};

            await _store.Save(Data.ActorId, _events, CancellationToken.None);

        }
        
        [Test]
        public void Should_have_added_or_updated_the_state()
        {
            A.CallTo(() => _stateManager.AddOrUpdateStateAsync(A<string>.Ignored, A<EventStream>.That.Matches(e => e.Contains(_events[0]) && e.Contains(_events[1])), A<Func<string,EventStream, EventStream>>.That.IsNotNull(), A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
        
        private static class Data
        {
            public static readonly Guid ActorId = Guid.NewGuid();
        }

    }
}