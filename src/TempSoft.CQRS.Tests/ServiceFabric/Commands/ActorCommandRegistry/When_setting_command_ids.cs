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
    public class When_setting_command_ids
    {
        private IActorStateManager _stateManager;
        private CQRS.ServiceFabric.Commands.ActorCommandRegistry _registry;
        private Guid[] _commandIds;
        private Func<string, Guid[], Guid[]> _callback;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _stateManager = A.Fake<IActorStateManager>();
            _commandIds = new[] {Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()};

            A.CallTo(() => _stateManager.AddOrUpdateStateAsync(A<string>.Ignored, A<Guid[]>.Ignored,
                    A<Func<string, Guid[], Guid[]>>.That.IsNotNull(), A<CancellationToken>.Ignored))
                .Invokes(fac => _callback = fac.GetArgument<Func<string, Guid[], Guid[]>>(2));


            _registry = new CQRS.ServiceFabric.Commands.ActorCommandRegistry(_stateManager);
            await _registry.Save(Data.ActorId, _commandIds, CancellationToken.None);
        }

        private static class Data
        {
            public static readonly Guid ActorId = Guid.NewGuid();
        }

        [Test]
        public void Should_have_specified_a_proper_merge_method()
        {
            var existing = new[] {Guid.NewGuid(), Guid.NewGuid()};

            var result = _callback(string.Empty, existing);

            result.Should().Contain(existing);
            result.Should().Contain(_commandIds);
        }

        [Test]
        public void Should_have_tried_to_set_the_state()
        {
            A.CallTo(() => _stateManager.AddOrUpdateStateAsync(A<string>.Ignored,
                    A<Guid[]>.That.Matches(ids =>
                        ids.Length == _commandIds.Length && ids.All(id => _commandIds.Contains(id))),
                    A<Func<string, Guid[], Guid[]>>.That.IsNotNull(), A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}