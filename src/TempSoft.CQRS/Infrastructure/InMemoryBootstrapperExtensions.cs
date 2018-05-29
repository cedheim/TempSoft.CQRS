using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.Infrastructure
{
    public static class InMemoryBootstrapperExtensions
    {
        public static FluentBootstrapper UseInMemoryCommandRouter(this FluentBootstrapper bootstrapper)
        {
            bootstrapper.UseService<ICommandRouter, InMemoryCommandRouter>(true);
            return bootstrapper;
        }

        public static FluentBootstrapper UseInMemoryCommandRegistry(this FluentBootstrapper bootstrapper)
        {
            bootstrapper.UseService<ICommandRegistry, InMemoryCommandRegistry>(true);
            return bootstrapper;
        }

        public static FluentBootstrapper UseInMemoryEventBus(this FluentBootstrapper bootstrapper)
        {
            bootstrapper.UseService<IEventBus, InMemoryEventBus>(true);
            return bootstrapper;
        }

        public static FluentBootstrapper UseInMemoryEventStore(this FluentBootstrapper bootstrapper)
        {
            bootstrapper.UseService<IEventStore, InMemoryEventStore>(true);
            return bootstrapper;
        }

    }
}