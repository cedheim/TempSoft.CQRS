﻿using System;
using FluentAssertions;
using NUnit.Framework;
using TempSoft.CQRS.Domain;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.Tests.Mocks;

namespace TempSoft.CQRS.Tests.Domain.AggregateRoot
{
    [TestFixture]
    public class When_invoking_an_entity_command
    {
        private AThingAggregateRoot _root;
        private Commit _commit;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _root = new AThingAggregateRoot();
            _root.Initialize(Data.RootId);
            _root.AddStuff(Data.EntityId, Data.StuffMessage);
            _root.Handle(new SetStuffMessage(Data.EntityId, Data.ChangedStuffMessage));

            _commit = _root.Commit();
        }

        [Test]
        public void Should_have_created_an_event()
        {
            _commit.Events.Should().ContainSingle(e => e is StuffMessageSet && ((StuffMessageSet)e).EntityId == Data.EntityId && ((StuffMessageSet)e).Message == Data.ChangedStuffMessage);
        }

        [Test]
        public void Should_have_changed_the_stuff_message()
        {
            _root.Stuff.Should().ContainSingle(stuff => stuff.Id == Data.EntityId && stuff.Message == Data.ChangedStuffMessage);
        }

        private static class Data
        {
            public static readonly Guid RootId = Guid.NewGuid();
            public static readonly Guid EntityId = Guid.NewGuid();
            public const string StuffMessage = "STUFF!!";
            public const string ChangedStuffMessage = "MOAR STUFF!!!!";
        }

    }
}