using System;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Microsoft.ServiceFabric.Actors;
using NUnit.Framework;
using TempSoft.CQRS.Mocks;
using TempSoft.CQRS.ServiceFabric.Domain;
using TempSoft.CQRS.ServiceFabric.Interfaces.Messaging;

namespace TempSoft.CQRS.ServiceFabric.Tests.Domain
{
    [TestFixture]
    public class When_handling_a_command : AggregateRootActorTestBase
    {
        private AggregateRootActor _actor;
        private AThingAggregateRoot _root;
        private ActorId _actorId;
        private DoSomething _command;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _root = new AThingAggregateRoot() {Id = Data.ActorId};
            _root.Initialize();
            _root.Commit();

            A.CallTo(() => AggregateRootRepository.Get(A<Type>.Ignored, A<string>.Ignored, A<bool>.Ignored, A<CancellationToken>.Ignored))
                .Returns(_root);

            _actorId = new ActorId(Data.ActorId);
            _actor = ActorService.Activate(_actorId);

            _command = new DoSomething(Data.AValue, Data.BValue);

            await _actor.Handle(new CommandMessage(typeof(AThingAggregateRoot), _command), CancellationToken.None);
        }

        private static class Data
        {
            public const int AValue = 5;
            public const string BValue = "TEST";
            public static readonly string ActorId = Guid.NewGuid().ToString();
        }

        [Test]
        public void Should_have_gotten_the_root_from_the_repository()
        {
            A.CallTo(() =>
                    AggregateRootRepository.Get(typeof(AThingAggregateRoot), Data.ActorId, true,
                        A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_have_saved_the_aggregate_to_the_repository()
        {
            A.CallTo(() => AggregateRootRepository.Save(_root, A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_have_set_the_values()
        {
            _root.A.Should().Be(Data.AValue);
            _root.B.Should().Be(Data.BValue);
        }
    }
}