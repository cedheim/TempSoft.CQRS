using System;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Microsoft.ServiceFabric.Actors;
using NUnit.Framework;
using TempSoft.CQRS.Mocks;
using TempSoft.CQRS.ServiceFabric.Interfaces.Messaging;
using TempSoft.CQRS.ServiceFabric.Interfaces.Projectors;
using TempSoft.CQRS.ServiceFabric.Projectors;

namespace TempSoft.CQRS.ServiceFabric.Tests.Projectors.ServiceFabricProjectionQueryRouters
{
    [TestFixture]
    public class When_querying_a_projector : ServiceFabricProjectionQueryRouterTestBase
    {
        private ServiceFabricProjectionQueryRouter _router;
        private AThingListResult _result;
        private QueryResultMessage _message;
        private AThingListQuery _query;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _message = new QueryResultMessage(new AThingListResult());
            _query = new AThingListQuery();

            A.CallTo(() => Actor.Query(A<QueryMessage>.Ignored, A<CancellationToken>.Ignored))
                .Returns(_message);

            _router = new ServiceFabricProjectionQueryRouter(ActorProxyFactory, UriHelper);
            _result = await _router.SendQuery<AThingProjector, AThingListResult>(_query, Data.ProjectId1, CancellationToken.None);
        }

        [Test]
        public void Should_have_created_an_uri()
        {
            A.CallTo(() => UriHelper.GetUriFor<IProjectorActor>())
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_have_created_an_actor_proxy()
        {
            A.CallTo(() => ActorProxyFactory.CreateActorProxy<IProjectorActor>(A<Uri>.That.IsNotNull(), A<ActorId>.That.Matches(id => id.GetStringId() == Data.ProjectId1), A<string>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }


        [Test]
        public void Should_have_called_the_projector_actor()
        {
            A.CallTo(() => Actor.Query(A<QueryMessage>.That.Matches(msg => object.ReferenceEquals(msg.Body, _query)), A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_have_returned_the_correct_result()
        {
            _result.Should().Be(_message.Body);
        }

        private static class Data
        {
            public const string ProjectId1 = "ProjectorId1";
        }


    }
}