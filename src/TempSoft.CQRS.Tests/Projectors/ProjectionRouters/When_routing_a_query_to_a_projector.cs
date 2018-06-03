using System;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using TempSoft.CQRS.InMemory.Projectors;
using TempSoft.CQRS.Mocks;
using TempSoft.CQRS.Projectors;

namespace TempSoft.CQRS.Tests.Projectors.ProjectionRouters
{
    public class When_routing_a_query_to_a_projector
    {
        private IProjectorRepository _projectorRepository;
        private IProjectionModelRepository _projectionRepository;
        private AThingProjector _projector;
        private AThingProjection _projection;
        private InMemoryProjectionQueryRouter _router;
        private AThingListResult _result;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _projectionRepository = A.Fake<IProjectionModelRepository>();
            _projectorRepository = A.Fake<IProjectorRepository>();

            _projector = new AThingProjector(_projectionRepository) {ProjectorId = Data.ProjectorId};
            _projection = new AThingProjection(Data.ProjectionId, Data.ProjectorId);

            A.CallTo(() => _projectionRepository.List(A<string>.Ignored, A<Func<IProjection, CancellationToken, Task>>.Ignored, A<CancellationToken>.Ignored))
                .Invokes(foc =>
                {
                    var token = foc.GetArgument<CancellationToken>(2);
                    var callback = foc.GetArgument<Func<IProjection, CancellationToken, Task>>(1);
                    callback(_projection, token).Wait();
                });
            A.CallTo(() => _projectorRepository.Get<AThingProjector>(A<string>.Ignored, A<CancellationToken>.Ignored))
                .Returns(_projector);

            _router = new InMemoryProjectionQueryRouter(_projectorRepository);
            _result = await _router.SendQuery<AThingProjector, AThingListResult>(new AThingListQuery(), Data.ProjectorId, CancellationToken.None);
        }

        [Test]
        public void Should_have_called_the_projector_repository()
        {
            A.CallTo(() => _projectorRepository.Get<AThingProjector>(Data.ProjectorId, A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_have_listed_projections()
        {
            A.CallTo(() => _projectionRepository.List(Data.ProjectorId, A<Func<IProjection, CancellationToken, Task>>.Ignored, A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_have_returned_the_correct_result()
        {
            _result.ProjectorId.Should().Be(Data.ProjectorId);
            _result.Projections.Should().OnlyContain(p => object.ReferenceEquals(p, _projection));
        }

        private static class Data
        {
            public static readonly string ProjectionId = Guid.NewGuid().ToString();
            public static readonly string ProjectorId = Guid.NewGuid().ToString();
        }
    }
}