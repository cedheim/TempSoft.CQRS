using System;
using System.Collections.Generic;
using TempSoft.CQRS.Demo.Domain.Movies.Events;
using TempSoft.CQRS.Demo.Projectors.MovieLists;
using TempSoft.CQRS.Infrastructure;

namespace TempSoft.CQRS.Demo.Infrastructure
{
    public static class BootstrapperGenerator
    {
        public static FluentBootstrapper Generate()
        {
            var bootstrapper = new FluentBootstrapper();

            bootstrapper.UseProjector<MovieList>("MovieList", "MovieList", new[] {typeof(MovieCreated)});

            return bootstrapper;
        }

    }
}
