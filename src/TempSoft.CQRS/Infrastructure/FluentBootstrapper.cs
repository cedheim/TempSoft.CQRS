using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Domain;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.Exceptions;
using TempSoft.CQRS.Projectors;
using TinyIoC;

namespace TempSoft.CQRS.Infrastructure
{
    public class FluentBootstrapper : IDisposable, IServiceProvider
    {
        private readonly IProjectorRegistry _projectorRegistry;
        private readonly TinyIoCContainer _container;

        public FluentBootstrapper()
        {
            this._container = new TinyIoCContainer();
            this._projectorRegistry = new ProjectorRegistry();

            // default registration.
            this._container.Register(this);
            this._container.Register<IServiceProvider>(this);
            this._container.Register<IAggregateRootRepository, AggregateRootRepository>().AsSingleton();
            this._container.Register<IProjectorRegistry>(_projectorRegistry);
            this._container.Register<IProjectorRepository, ProjectorRepository>().AsSingleton();
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
            var registration = this._container.Register<TServiceInterface, TServiceImplementation>();
            if (singleton)
            {
                registration.AsSingleton();
            }

            return this;
        }

        public FluentBootstrapper UseService<TServiceInterface>(TServiceInterface instance)
            where TServiceInterface : class
        {
            this._container.Register<TServiceInterface>(instance);

            return this;
        }

        public FluentBootstrapper UseService<TServiceInterface>(Func<TServiceInterface> factory, bool singleton = false)
            where TServiceInterface : class
        {
            var registration = this._container.Register<TServiceInterface>((container, overloads) => factory());
            if (singleton)
            {
                registration.AsSingleton();
            }

            return this;
        }

        public TService Resolve<TService>() where TService : class
        {
            return _container.Resolve<TService>();
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

        public void Dispose()
        {
            this._container.Dispose();
        }

        object IServiceProvider.GetService(Type serviceType)
        {
            return _container.Resolve(serviceType);
        }

        private void ValidateServiceResolution<TService>(ICollection<ValidationFailure> failures) where TService : class
        {
            if (!_container.CanResolve<TService>())
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
        
    }
}
