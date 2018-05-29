using NUnit.Framework;
using TempSoft.CQRS.Infrastructure;
using TempSoft.CQRS.Mocks;

namespace TempSoft.CQRS.Tests.Infrastructure
{
    [TestFixture]
    public class When_working_with_in_memory_services
    {
        private FluentBootstrapper _bootstrapper;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _bootstrapper = new FluentBootstrapper();
            _bootstrapper.UseInMemoryCommandRegistry()
                .UseInMemoryCommandRouter()
                .UseInMemoryEventBus()
                .UseInMemoryEventStore()
                .UseProjector<AThingProjector>(nameof(AThingProjector))
                .Validate();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _bootstrapper.Dispose();
        }

        [Test]
        public void Fail()
        {


            Assert.Fail();
        }

        
    }
}