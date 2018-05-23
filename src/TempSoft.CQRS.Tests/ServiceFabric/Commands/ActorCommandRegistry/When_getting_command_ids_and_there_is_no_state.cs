using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Microsoft.ServiceFabric.Actors.Runtime;
using NUnit.Framework;

namespace TempSoft.CQRS.Tests.ServiceFabric.Commands.ActorCommandRegistry
{
    [TestFixture]
    public class When_getting_command_ids_and_there_is_no_state
    {
        private IActorStateManager _stateManager;
        private CQRS.ServiceFabric.Commands.ActorCommandRegistry _registry;
        private Guid[] _commandIds;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _stateManager = A.Fake<IActorStateManager>();

            _registry = new CQRS.ServiceFabric.Commands.ActorCommandRegistry(_stateManager);
            _commandIds = (await _registry.Get(Data.ActorId, CancellationToken.None)).ToArray();
        }

        private static class Data
        {
            public static readonly Guid ActorId = Guid.NewGuid();
        }

        [Test]
        public void Should_have_returned_an_empty_collection()
        {
            _commandIds.Should().NotBeNull();
            _commandIds.Should().BeEmpty();
        }

        [Test]
        public void Should_have_tried_to_get_the_state()
        {
            A.CallTo(() =>
                    _stateManager.TryGetStateAsync<Guid[]>(A<string>.That.IsNotNull(), A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}