﻿using System;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Microsoft.ServiceFabric.Actors;
using NUnit.Framework;
using TempSoft.CQRS.ServiceFabric.Domain;
using TempSoft.CQRS.ServiceFabric.Interfaces.Messaging;
using TempSoft.CQRS.Tests.Mocks;

namespace TempSoft.CQRS.Tests.ServiceFabric.Domain
{
    [TestFixture]
    public class When_getting_the_read_model : AggregateRootActorTestBase
    {
        private AggregateRootActor _actor;
        private AThingAggregateRoot _root;
        private ActorId _actorId;
        private DoSomething _command;
        private ReadModelMessage _readModelMessage;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _root = new AThingAggregateRoot();
            _root.Initialize(Data.ActorId);
            _root.DoSomething(Data.AValue, Data.BValue);
            _root.Commit();

            A.CallTo(() => AggregateRootRepository.Get(A<Type>.Ignored, A<Guid>.Ignored, A<CancellationToken>.Ignored))
                .Returns(_root);

            _actorId = new ActorId(Data.ActorId);
            _actor = ActorService.Activate(_actorId);

            _readModelMessage = await _actor.GetReadModel(new GetReadModelMessage(typeof(AThingAggregateRoot)), CancellationToken.None);
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
        public void Should_have_returned_the_correct_read_model()
        {
            var readModel = _readModelMessage.GetReadModel<AThingReadModel>();

            readModel.Should().BeEquivalentTo(_root);
        }
        
        private static class Data
        {
            public static readonly Guid ActorId = Guid.NewGuid();
            public const int AValue = 5;
            public const string BValue = "TEST";
        }
    }
}