using TempSoft.CQRS.Demo.Domain.Movies.Events;
using TempSoft.CQRS.Demo.Domain.Movies.Projectors;
using TempSoft.CQRS.Infrastructure;

namespace TempSoft.CQRS.Demo.Infrastructure
{
    public static class BootstrapperGenerator
    {
        public static FluentBootstrapper Generate()
        {
            var bootstrapper = new FluentBootstrapper();
            bootstrapper.UseProjector<MovieListProjector>(nameof(MovieListProjector), nameof(MovieListProjector), new []{ typeof(MovieInitialized) });

            return bootstrapper;
        }

    }
}
