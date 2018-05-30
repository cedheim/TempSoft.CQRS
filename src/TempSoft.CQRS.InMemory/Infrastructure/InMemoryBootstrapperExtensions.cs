using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.Infrastructure;
using TempSoft.CQRS.InMemory.Commands;
using TempSoft.CQRS.InMemory.Events;
using TempSoft.CQRS.InMemory.Projectors;
using TempSoft.CQRS.Projectors;

namespace TempSoft.CQRS.InMemory.Infrastructure
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

        public static FluentBootstrapper UseInMemoryProjectionModelRepository(this FluentBootstrapper bootstrapper)
        {
            bootstrapper.UseService<IProjectionModelRepository, InMemoryProjectionModelRepository>(true);
            return bootstrapper;
        }

    }
}