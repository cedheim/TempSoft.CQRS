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
    public class FluentBootstrapper : IDisposable, IServiceProvider
    {
        private readonly IProjectorRegistry _projectorRegistry;
        private readonly IServiceLocator _locator;

        public FluentBootstrapper(IServiceLocator locator = default(IServiceLocator))
        {
            this._locator = locator ?? new ServiceLocator();
            this._projectorRegistry = new ProjectorRegistry();

            // default registration.
            this._locator.Register(this);
            this._locator.Register<IServiceProvider>(this);
            this._locator.Register<IServiceLocator>(this._locator);
this._locator.Register<IAggregateRootRepository, AggregateRootRepository>().AsSingleton();
            this._locator.Register<IProjectorRegistry>(_projectorRegistry);
            this._locator.Register<IProjectorRepository, ProjectorRepository>().AsSingleton();
        }

        public FluentBootstrapper UseProjector<TProjector>(string name, string identifiedBy = default(string), IEnumerable<Type> eventTypes = default(IEnumerable<Type>), IEnumerable<string> eventGroups = default(IEnumerable<string>)) where TProjector : IProjector
        {
            if(!typeof(TProjector).IsClass || typeof(TProjector).IsAbstract)
                throw new BootstrapperProjectorException($"Projector type needs to be a proper class and not abstract ({typeof(TProjector).Name}).");

            identifiedBy = identifiedBy ?? name;

            _projectorRegistry.Register(new ProjectorDefinition(name, identifiedBy, typeof(TProjector), eventTypes, eventGroups));

            return this;
        }

        public FluentBootstrapper UseService<TServiceInterface, TServiceImplementation>(bool singleton = false) where TServiceInterface : class where TServiceImplementation : class, TServiceInterface
        {
            var registration = this._locator.Register<TServiceInterface, TServiceImplementation>();
            if (singleton)
            {
                registration.AsSingleton();
            }

            return this;
        }

        public FluentBootstrapper UseService<TServiceInterface>(TServiceInterface instance)
            where TServiceInterface : class
        {
            this._locator.Register<TServiceInterface>(instance);

            return this;
        }

        public FluentBootstrapper UseService<TServiceInterface>(Func<TServiceInterface> factory, bool singleton = false)
            where TServiceInterface : class
        {
            var registration = this._locator.Register<TServiceInterface>(factory);
            if (singleton)
            {
                registration.AsSingleton();
            }

            return this;
        }

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
            if (!_locator.CanResolve<TService>())
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
            this._locator.Dispose();
        }

        object IServiceProvider.GetService(Type serviceType)
        {
            return _locator.Resolve(serviceType);
        }
    }
}
