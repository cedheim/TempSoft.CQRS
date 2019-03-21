using System;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using NUnit.Framework;
using TempSoft.CQRS.Mocks;
using TempSoft.CQRS.Projectors;
using TempSoft.CQRS.Tests.Domain.AggregateRoot;

namespace TempSoft.CQRS.Tests.Projectors
{
    [TestFixture]
    public class When_projecting_an_event
    {
        private IProjectionModelRepository _repository;
        private AThingProjector _projector;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _repository = A.Fake<IProjectionModelRepository>();
            _projector = new AThingProjector(_repository)
            {
                ProjectorId = Data.ProjectorId
            };

            await _projector.Project(new CreatedAThing(){AggregateRootId = Data.AggregateRootId }, CancellationToken.None);
        }

        [Test]
        public void Should_have_stored_the_projection()
        {
            A.CallTo(() => _repository.Save(A<AThingProjection>.That.Matches(projection => projection.Id == Data.ProjectionId), A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }


        private static class Data
        {
            public const string ProjectorId = nameof(AThingProjection);
            public static readonly string AggregateRootId = Guid.NewGuid().ToString();
            public static readonly string ProjectionId = $"{ProjectorId}_{AggregateRootId}";
        }
    }
}