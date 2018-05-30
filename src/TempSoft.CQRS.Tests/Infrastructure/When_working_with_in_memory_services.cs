using FakeItEasy;
using NUnit.Framework;
using TempSoft.CQRS.Infrastructure;
using TempSoft.CQRS.InMemory.Infrastructure;
using TempSoft.CQRS.Mocks;
using TempSoft.CQRS.Projectors;

namespace TempSoft.CQRS.Tests.Infrastructure
{
    [TestFixture]
    public class When_working_with_in_memory_services
    {
        private FluentBootstrapper _bootstrapper;
        private IProjectionModelRepository _repository;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _repository = A.Fake<IProjectionModelRepository>();
            _bootstrapper = new FluentBootstrapper();
            _bootstrapper.UseInMemoryCommandRegistry()
                .UseInMemoryCommandRouter()
                .UseInMemoryEventBus()
                .UseInMemoryEventStore()
                .UseService<IProjectionModelRepository>(_repository)
                .UseProjector<AThingProjector>(nameof(AThingProjector))
                .Validate();

        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _bootstrapper.Dispose();
        }



        
    }
}