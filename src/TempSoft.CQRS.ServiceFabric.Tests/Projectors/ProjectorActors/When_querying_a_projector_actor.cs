using System;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Microsoft.ServiceFabric.Actors;
using NUnit.Framework;
using TempSoft.CQRS.Mocks;
using TempSoft.CQRS.Projectors;
using TempSoft.CQRS.ServiceFabric.Interfaces.Messaging;
using TempSoft.CQRS.ServiceFabric.Projectors;

namespace TempSoft.CQRS.ServiceFabric.Tests.Projectors.ProjectorActors
{
    [TestFixture]
    public class When_querying_a_projector_actor : ProjectorActorTestBase
    {
        private AThingProjector _projector;
        private QueryMessage _message;
        private ActorId _actorId;
        private ProjectorActor _actor;
        private AThingProjection _projection;
        private AThingListQuery _query;
        private QueryResultMessage _result;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _projection = new AThingProjection(Data.ProjectionId, Data.ProjectorId);
            _projector = new AThingProjector(ProjectionModelRepository) { ProjectorId = Data.ProjectorId };
            _query = new AThingListQuery();
            _message = new QueryMessage(_query, typeof(AThingProjector));
            _actorId = new ActorId(Data.ProjectorId);

            A.CallTo(() => ProjectorRepository.Get(A<Type>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored))
                .Returns(_projector);
            A.CallTo(() => ProjectionModelRepository.List(A<string>.Ignored, A<Func<IProjection, CancellationToken, Task>>.Ignored, A<CancellationToken>.Ignored))
                .Invokes(foc =>
                {
                    var callback = foc.GetArgument<Func<IProjection, CancellationToken, Task>>(1);
                    var token = foc.GetArgument<CancellationToken>(2);
                    callback(_projection, token).Wait();
                });

            _actor = ActorService.Activate(_actorId);
            _result = await _actor.Query(_message, CancellationToken.None);
        }

        [Test]
        public void Should_have_gotten_the_projector()
        {
            A.CallTo(() => ProjectorRepository.Get(typeof(AThingProjector), Data.ProjectorId, A<CancellationToken>.Ignored))
                .MustHaveHappened();
        }

        [Test]
        public void Should_have_listed_projections()
        {
            A.CallTo(() => ProjectionModelRepository.List(A<string>.Ignored, A<Func<IProjection, CancellationToken, Task>>.Ignored, A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_have_returned_the_correct_result()
        {
            var result = _result.GetQueryResult<AThingListResult>();
            result.ProjectorId.Should().Be(Data.ProjectorId);
            result.Projections.Should().ContainSingle(p => object.ReferenceEquals(p, _projection));
        }

        private static class Data
        {
            public static readonly Guid AggregateRootId = Guid.NewGuid();
            public static readonly string ProjectionId = $"{Data.ProjectorId}_{AggregateRootId}";
            public const string ProjectorId = "Projector";
        }
    }
}