using System.Threading.Tasks;
using FakeItEasy;
using NCG.NGS.CQRS.Commands;
using NCG.NGS.CQRS.Events;
using NUnit.Framework;

namespace NCG.NGS.CQRS.Tests.Domain
{
    [TestFixture]
    public class When_getting_an_aggregate_from_the_repository
    {
        private IEventStore _eventStore;
        private IEventBus _eventBus;
        private ICommandRegistry _commandRegistry;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _eventStore = A.Fake<IEventStore>();
            _eventBus = A.Fake<IEventBus>();
            _commandRegistry = A.Fake<ICommandRegistry>();
        }
        
    }
}