using FluentAssertions;
using NUnit.Framework;
using TempSoft.CQRS.Infrastructure;
using TempSoft.CQRS.Mocks;

namespace TempSoft.CQRS.Tests.Infrastructure
{
    [TestFixture]
    public class When_registering_and_resolving_services
    {
        public interface IExampleService
        {
        }

        public class ExampleService1 : IExampleService
        {
        }

        public class ExampleService2 : IExampleService
        {
        }

        [Test]
        public void Should_be_able_register_by_name()
        {
            Services.Register<IExampleService, ExampleService1>();
            Services.Register<IExampleService, ExampleService2>("service2");

            var service1 = Services.Resolve<IExampleService>();
            var service2 = Services.Resolve<IExampleService>("service2");

            service1.Should().NotBeNull();
            service1.Should().BeOfType<ExampleService1>();

            service2.Should().NotBeNull();
            service2.Should().BeOfType<ExampleService2>();
        }

        [Test]
        public void Should_be_able_to_override_a_registration()
        {
            Services.Register<IExampleService, ExampleService1>();
            Services.Register<IExampleService, ExampleService2>();

            var service = Services.Resolve<IExampleService>();

            service.Should().NotBeNull();
            service.Should().BeOfType<ExampleService2>();
        }

        [Test]
        public void Should_be_able_to_resolve_an_object_by_interface()
        {
            Services.Register<IExampleService, ExampleService1>();

            var service = Services.Resolve<IExampleService>();

            service.Should().NotBeNull();
            service.Should().BeOfType<ExampleService1>();
        }

        [Test]
        public void Should_be_able_to_resolve_new_objects()
        {
            var root = Services.Resolve<AThingAggregateRoot>();
            root.Should().NotBeNull();
        }
    }
}