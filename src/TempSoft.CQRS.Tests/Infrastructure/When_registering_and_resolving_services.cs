using FluentAssertions;
using NUnit.Framework;
using TempSoft.CQRS.Infrastructure;
using TempSoft.CQRS.Mocks;

namespace TempSoft.CQRS.Tests.Infrastructure
{
    [TestFixture]
    public class When_registering_and_resolving_services
    {
        private ServiceLocator _services;

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
            _services = new ServiceLocator();
        }

        [Test]
        public void Should_be_able_register_by_name()
        {
            _services.Register<IExampleService, ExampleService1>();
            _services.Register<IExampleService, ExampleService2>("service2");

            var service1 = _services.Resolve<IExampleService>();
            var service2 = _services.Resolve<IExampleService>("service2");

            service1.Should().NotBeNull();
            service1.Should().BeOfType<ExampleService1>();

            service2.Should().NotBeNull();
            service2.Should().BeOfType<ExampleService2>();
        }

        [Test]
        public void Should_be_able_to_override_a_registration()
        {
            _services.Register<IExampleService, ExampleService1>();
            _services.Register<IExampleService, ExampleService2>();

            var service = _services.Resolve<IExampleService>();

            service.Should().NotBeNull();
            service.Should().BeOfType<ExampleService2>();
        }

        [Test]
        public void Should_be_able_to_resolve_an_object_by_interface()
        {
            _services.Register<IExampleService, ExampleService1>();

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