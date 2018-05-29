using FluentAssertions;
using NUnit.Framework;
using TempSoft.CQRS.Infrastructure;
using TempSoft.CQRS.Mocks;

namespace TempSoft.CQRS.Tests.Infrastructure
{
    [TestFixture]
    public class When_registering_and_resolving_services
    {
        private FluentBootstrapper _services;

        public interface IExampleService
        {
        }

        public class ExampleService1 : IExampleService
        {
        }

        public class ExampleService2 : IExampleService
        {
        }

        [SetUp]
        public void SetUp()
        {
            _services = new FluentBootstrapper();
        }
        
        [Test]
        public void Should_be_able_to_override_a_registration()
        {
            _services.UseService<IExampleService, ExampleService1>();
            _services.UseService<IExampleService, ExampleService2>();

            var service = _services.Resolve<IExampleService>();

            service.Should().NotBeNull();
            service.Should().BeOfType<ExampleService2>();
        }

        [Test]
        public void Should_be_able_to_resolve_an_object_by_interface()
        {
            _services.UseService<IExampleService, ExampleService1>();

            var service = _services.Resolve<IExampleService>();

            service.Should().NotBeNull();
            service.Should().BeOfType<ExampleService1>();
        }

        [Test]
        public void Should_be_able_to_resolve_new_objects()
        {
            var root = _services.Resolve<AThingAggregateRoot>();
            root.Should().NotBeNull();
        }
    }
}