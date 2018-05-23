using System.Linq;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using TempSoft.CQRS.Queries;
using TempSoft.CQRS.Tests.Mocks;

namespace TempSoft.CQRS.Tests.Queries
{
    [TestFixture]
    public class When_registering_a_query_builder
    {
        private QueryBuilderRegistry _registry;
        private AThingQueryBuilder _builder;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _builder = new AThingQueryBuilder(A.Fake<IQueryModelRepository>());
            _registry = new QueryBuilderRegistry();
            _registry.Register(_builder);
        }

        [Test]
        public void Should_be_able_to_get_the_query_builder_by_name()
        {
            var registeredBuilder = _registry.GetQueryBuilderByType(typeof(AThingQueryBuilder));

            registeredBuilder.Should().BeSameAs(_builder);
        }

        [Test]
        public void Should_include_the_query_builder_when_listing()
        {
            var listingA = _registry.ListQueryBuildersFor(typeof(ChangedAValue)).ToArray();
            listingA.Should().Contain(b => ReferenceEquals(_builder, b));

            var listingB = _registry.ListQueryBuildersFor(typeof(ChangedBValue)).ToArray();
            listingB.Should().Contain(b => ReferenceEquals(_builder, b));
        }
    }
}