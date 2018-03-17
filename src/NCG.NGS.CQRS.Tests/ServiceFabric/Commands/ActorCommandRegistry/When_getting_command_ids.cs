using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Data;
using NUnit.Framework;

namespace NCG.NGS.CQRS.Tests.ServiceFabric.Commands.ActorCommandRegistry
{
    [TestFixture]
    public class When_getting_command_ids
    {
        private IActorStateManager _stateManager;
        private CQRS.ServiceFabric.Commands.ActorCommandRegistry _registry;
        private Guid[] _result;
        private Guid[] _commandIds;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _stateManager = A.Fake<IActorStateManager>();
            _commandIds = new[] {Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()};

            A.CallTo(() => _stateManager.TryGetStateAsync<Guid[]>(A<string>.Ignored, A<CancellationToken>.Ignored))
                .Returns(new ConditionalValue<Guid[]>(true, _commandIds));

            _registry = new CQRS.ServiceFabric.Commands.ActorCommandRegistry(_stateManager);
            _result = (await _registry.Get(Data.ActorId, CancellationToken.None)).ToArray();
        }

        [Test]
        public void Should_have_tried_to_get_the_state()
        {
            A.CallTo(() => _stateManager.TryGetStateAsync<Guid[]>(A<string>.That.IsNotNull(), A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_have_returned_an_empty_collection()
        {
            _result.Should().BeEquivalentTo(_commandIds);
        }

        private static class Data
        {
            public static readonly Guid ActorId = Guid.NewGuid();
        }

    }
}