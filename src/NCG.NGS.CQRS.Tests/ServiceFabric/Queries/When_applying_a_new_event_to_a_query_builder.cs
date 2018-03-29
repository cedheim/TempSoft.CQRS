using System;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using NCG.NGS.CQRS.Common.Extensions;
using NCG.NGS.CQRS.Queries;
using NCG.NGS.CQRS.ServiceFabric.Interfaces.Messaging;
using NCG.NGS.CQRS.ServiceFabric.Queries;
using NCG.NGS.CQRS.Tests.Mocks;
using NUnit.Framework;
using ServiceFabric.Mocks;

namespace NCG.NGS.CQRS.Tests.ServiceFabric.Queries
{
    [TestFixture]
    public class When_applying_a_new_event_to_a_query_builder : QueryBuilderActorTestBase
    {
        private IQueryBuilder _queryBuilder;
        private CreatedAThing _event;
        private QueryBuilderActor _actor;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _queryBuilder = A.Fake<IQueryBuilder>();
            _event = new CreatedAThing(Guid.NewGuid());

            A.CallTo(() => Registry.GetQueryBuilderByType(A<Type>.Ignored))
                .Returns(_queryBuilder);

            _actor = ActorService.Activate(new ActorId(Data.QueryBuilderFriendlyName));
            await _actor.InvokeOnActivateAsync();

            await _actor.Apply(new EventMessage(_event), CancellationToken.None);
        }

        [Test]
        public void Should_have_tried_to_get_the_query_builder()
        {
            A.CallTo(() => Registry.GetQueryBuilderByType(A<Type>.That.Matches(t => t == typeof(AThingQueryBuilder))))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_have_applied_the_event_to_the_query_builder()
        {
            A.CallTo(() => _queryBuilder.Apply(_event, A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        private static class Data
        {
            public static readonly string QueryBuilderFriendlyName = typeof(AThingQueryBuilder).ToFriendlyName();
        }
    }
}