using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Domain;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.Exceptions;
using TempSoft.CQRS.Projectors;

namespace TempSoft.CQRS.Infrastructure
{
    public class FluentBootstrapper : IDisposable
    {
        public FluentBootstrapper(IServiceLocator locator = default(IServiceLocator))
        {
            this.Locator = locator ?? new ServiceLocator();

            // default registration.
            this.Locator.Register(this);
            this.Locator.Register(this.Locator);
            this.Locator.Register<IServiceProvider>(this.Locator);
        }

        public IServiceLocator Locator { get; }

        public FluentBootstrapper Validate()
        {
            var failures = new List<ValidationFailure>();
            ValidateServiceResolution<ICommandRouter>(failures);
            ValidateServiceResolution<ICommandRegistry>(failures);
            ValidateServiceResolution<IAggregateRootRepository>(failures);
            ValidateServiceResolution<IEventBus>(failures);
            ValidateServiceResolution<IEventStore>(failures);
            ValidateServiceResolution<IProjectorRegistry>(failures);
            ValidateServiceResolution<IProjectorRepository>(failures);

            if (failures.Count > 0)
            {
                throw new BootstrapperValidationException(failures.Select(failure => failure.ServiceType).ToArray());
            }

            return this;
        }

        private void ValidateServiceResolution<TService>(ICollection<ValidationFailure> failures) where TService : class
        {
            if (!Locator.CanResolve<TService>())
            {
                failures.Add(new ValidationFailure(typeof(TService)));
            }
        }

        private class ValidationFailure
        {
            public ValidationFailure(Type serviceType)
            {
                ServiceType = serviceType;
            }

            public Type ServiceType { get; }
        }

        public void Dispose()
        {
            this.Locator.Dispose();
        }
    }
}
