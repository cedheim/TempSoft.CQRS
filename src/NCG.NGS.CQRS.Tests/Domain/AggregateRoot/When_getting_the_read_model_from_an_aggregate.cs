using System;
using FluentAssertions;
using NCG.NGS.CQRS.Events;
using NCG.NGS.CQRS.Tests.Mocks;
using NUnit.Framework;

namespace NCG.NGS.CQRS.Tests.Domain.AggregateRoot
{
    [TestFixture]
    public class When_getting_the_read_model_from_an_aggregate
    {
        private AThingAggregateRoot _root;
        private AThingReadModel _readModel;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _root = new AThingAggregateRoot();
            _root.Initialize(Data.RootId);
            _root.Handle(new DoSomething(Data.AValue, Data.BValue));
            _root.Commit();

            _readModel = _root.GetReadModel() as AThingReadModel;
        }

        [Test]
        public void Should_have_set_fields()
        {
            _readModel.A.Should().Be(Data.AValue);
            _readModel.B.Should().Be(Data.BValue);
            _readModel.Id.Should().Be(Data.RootId);
            _readModel.Version.Should().Be(3);
        }

        private static class Data
        {
            public static readonly Guid RootId = Guid.NewGuid();
            public const int AValue = 5;
            public const string BValue = "FLEUF";
        }

    }
}