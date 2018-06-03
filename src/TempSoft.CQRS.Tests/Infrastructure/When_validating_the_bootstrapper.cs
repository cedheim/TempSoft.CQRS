using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Internal;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Domain;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.Infrastructure;
using TempSoft.CQRS.Exceptions;
using TempSoft.CQRS.InMemory.Infrastructure;
using TempSoft.CQRS.Projectors;

namespace TempSoft.CQRS.Tests.Infrastructure
{
    [TestFixture]
    public class When_validating_the_bootstrapper
    {
        private FluentBootstrapper _bootstrapper;

        [SetUp]
        public void SetUp()
        {
            _bootstrapper = new FluentBootstrapper();
        }

        [TearDown]
        public void TearDown()
        {
            _bootstrapper.Dispose();
        }

        [Test]
        public void Should_throw_an_exception_if_not_all_services_are_specified()
        {
            _bootstrapper.Invoking(b => b.Validate()).Should().Throw<BootstrapperValidationException>().Which.Should().Match<BootstrapperValidationException>(ex => 
                ex.MissingServices.Contains(typeof(ICommandRouter)) &&
                ex.MissingServices.Contains(typeof(ICommandRegistry)) &&
                ex.MissingServices.Contains(typeof(IEventBus)) &&
                ex.MissingServices.Contains(typeof(IEventStore)) &&
                ex.MissingServices.Contains(typeof(IProjectionModelRepository)) &&
                ex.MissingServices.Contains(typeof(IProjectionQueryRouter))
            );
        }

        [Test]
        public void Should_not_throw_an_exception_if_all_services_are_registered()
        {
            _bootstrapper.UseInMemoryCommandRegistry()
                .UseInMemoryCommandRouter()
                .UseInMemoryEventBus()
                .UseInMemoryEventStore()
                .UseInMemoryProjectionModelRepository()
                .UseInMemoryProjectionQueryRouter()
                .Validate();
        }

    }
}