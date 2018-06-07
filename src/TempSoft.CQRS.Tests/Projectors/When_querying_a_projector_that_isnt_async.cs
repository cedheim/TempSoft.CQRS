using System;
using System.Linq;
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
    public class When_querying_a_projector_that_isnt_async
    {
        private IProjectionModelRepository _repository;
        private AThingProjector _projector;
        private AThingListResult _result;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _repository = A.Fake<IProjectionModelRepository>();

            _projector = new AThingProjector(_repository)
            {
                ProjectorId = Data.ProjectorId
            };

            _result = (AThingListResult)await _projector.Query(new AThingEmptyQuery(), CancellationToken.None);
        }
        
        [Test]
        public void Should_have_returned_the_correct_result()
        {
            _result.Projections.Should().BeEmpty();
        }

        private static class Data
        {
            public const string ProjectorId = nameof(AThingProjection);
            public static readonly Guid AggregateRootId = Guid.NewGuid();
            public static readonly string ProjectionId = $"{ProjectorId}_{AggregateRootId}";
        }
    }
}