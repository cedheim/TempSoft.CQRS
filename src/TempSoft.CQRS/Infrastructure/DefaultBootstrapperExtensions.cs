using System.Collections;
using System.Collections.Generic;
using TempSoft.CQRS.Domain;
using TempSoft.CQRS.Projectors;

namespace TempSoft.CQRS.Infrastructure
{
    public static class DefaultBootstrapperExtensions
    {
        public static FluentBootstrapper UseAggregateRootRepository(this FluentBootstrapper bootstrapper)
        {
            bootstrapper.Locator.Register<IAggregateRootRepository, AggregateRootRepository>().AsSingleton();
            return bootstrapper;
        }

        public static FluentBootstrapper UseProjectorRegistry(this FluentBootstrapper bootstrapper, IEnumerable<ProjectorDefinition> definitions)
        {
            var registry = new ProjectorRegistry();
            foreach (var definition in definitions)
                registry.Register(definition);

            bootstrapper.Locator.Register<IProjectorRegistry>(registry);
            return bootstrapper;
        }

        public static FluentBootstrapper UseProjectorRepository(this FluentBootstrapper bootstrapper)
        {
            bootstrapper.Locator.Register<IProjectorRepository, ProjectorRepository>();
            return bootstrapper;
        }

    }
}