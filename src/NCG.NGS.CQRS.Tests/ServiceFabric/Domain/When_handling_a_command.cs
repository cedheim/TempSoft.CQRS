using System;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Microsoft.ServiceFabric.Actors;
using NCG.NGS.CQRS.ServiceFabric.Domain;
using NCG.NGS.CQRS.ServiceFabric.Events;
using NCG.NGS.CQRS.ServiceFabric.Interfaces.Messaging;
using NCG.NGS.CQRS.Tests.Mocks;
using NUnit.Framework;
using ServiceFabric.Mocks;

namespace NCG.NGS.CQRS.Tests.ServiceFabric.Domain
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
            _root = new AThingAggregateRoot();
            _root.Initialize(Data.ActorId);
            _root.Commit();

            A.CallTo(() => AggregateRootRepository.Get(A<Type>.Ignored, A<Guid>.Ignored, A<CancellationToken>.Ignored))
                .Returns(_root);

            _actorId = new ActorId(Data.ActorId);
            _actor = ActorService.Activate(_actorId);
            
            _command = new DoSomething(Data.AValue, Data.BValue);
            
            await _actor.Handle(new CommandMessage(typeof(AThingAggregateRoot), _command), CancellationToken.None);
        }

        [Test]
        public void Should_have_set_the_values()
        {
            _root.A.Should().Be(Data.AValue);
            _root.B.Should().Be(Data.BValue);
        }
        
        [Test]
        public void Should_have_gotten_the_root_from_the_repository()
        {
            A.CallTo(() => AggregateRootRepository.Get(typeof(AThingAggregateRoot), Data.ActorId, A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_have_saved_the_aggregate_to_the_repository()
        {
            A.CallTo(() => AggregateRootRepository.Save(_root, A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
        
        private static class Data
        {
            public static readonly Guid ActorId = Guid.NewGuid();
            public const int AValue = 5;
            public const string BValue = "TEST";
        }
    }
}