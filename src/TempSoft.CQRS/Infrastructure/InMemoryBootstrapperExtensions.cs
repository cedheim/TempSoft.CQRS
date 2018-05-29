using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.Infrastructure
{
    public static class InMemoryBootstrapperExtensions
    {
        public static FluentBootstrapper UseInMemoryCommandRouter(this FluentBootstrapper bootstrapper)
        {
            bootstrapper.Locator.Register<ICommandRouter, InMemoryCommandRouter>().AsSingleton();
            return bootstrapper;
        }

        public static FluentBootstrapper UseInMemoryCommandRegistry(this FluentBootstrapper bootstrapper)
        {
            bootstrapper.Locator.Register<ICommandRegistry, InMemoryCommandRegistry>().AsSingleton();
            return bootstrapper;
        }

        public static FluentBootstrapper UseInMemoryEventBus(this FluentBootstrapper bootstrapper)
        {
            bootstrapper.Locator.Register<IEventBus, InMemoryEventBus>().AsSingleton();
            return bootstrapper;
        }

        public static FluentBootstrapper UseInMemoryEventStore(this FluentBootstrapper bootstrapper)
        {
            bootstrapper.Locator.Register<IEventStore, InMemoryEventStore>().AsSingleton();
            return bootstrapper;
        }

    }
}