using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TempSoft.CQRS.Infrastructure;
using TempSoft.CQRS.Mocks;
using TempSoft.CQRS.Projectors;

namespace TempSoft.CQRS.Tests.Projectors.ProjectorRepositories
{
    [TestFixture]
    public class When_getting_a_projector_from_the_repository
    {
        private ProjectorRepository _repository;
        private IServiceProvider _serviceProvider;

        [SetUp]
        public void SetUp()
        {
            _serviceProvider = new ServicesLocator();
            _repository = new ProjectorRepository(_serviceProvider);
        }

        [Test]
        public async Task Should_be_able_to_get_projector_from_the_repository()
        {
            var projector = await _repository.Get<AThingProjector>("an id", CancellationToken.None);

            projector.Should().NotBeNull();
            projector.ProjectorId.Should().Be("an id");
        }
    }
}