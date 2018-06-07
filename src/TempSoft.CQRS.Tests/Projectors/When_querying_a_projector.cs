using System;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using TempSoft.CQRS.Mocks;
using TempSoft.CQRS.Projectors;
using TempSoft.CQRS.Tests.Domain.AggregateRoot;

namespace TempSoft.CQRS.Tests.Projectors
{
    [TestFixture]
    public class When_querying_a_projector
    {
        private IProjectionModelRepository _repository;
        private AThingProjector _projector;
        private AThingProjection _projection;
        private AThingListResult _result;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _repository = A.Fake<IProjectionModelRepository>();
            _projection = new AThingProjection(Data.ProjectionId, Data.ProjectorId);

            A.CallTo(() => _repository.List(A<string>.Ignored, A<Func<IProjection, CancellationToken, Task>>.Ignored, A<CancellationToken>.Ignored))
                .Invokes(foc =>
                {
                    var token = foc.GetArgument<CancellationToken>(2);
                    var callback = foc.GetArgument<Func<IProjection, CancellationToken, Task>>(1);
                    callback(_projection, token).Wait();
                });

            _projector = new AThingProjector(_repository)
            {
                ProjectorId = Data.ProjectorId
            };

            _result = (AThingListResult)await _projector.Query(new AThingListQuery(), CancellationToken.None);
        }

        [Test]
        public void Should_have_listed_projections()
        {
            A.CallTo(() => _repository.List(A<string>.Ignored, A<Func<IProjection, CancellationToken, Task>>.Ignored, A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_have_returned_the_correct_result()
        {
            _result.Projections.Should().ContainSingle(p => object.ReferenceEquals(p, _projection));
        }

        private static class Data
        {
            public const string ProjectorId = nameof(AThingProjection);
            public static readonly Guid AggregateRootId = Guid.NewGuid();
            public static readonly string ProjectionId = $"{ProjectorId}_{AggregateRootId}";
        }
    }
}