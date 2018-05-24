using System;
using System.Collections.Generic;
using System.Reflection;
using TinyIoC;

namespace TempSoft.CQRS.Infrastructure
{
    public class ServicesLocator : IServiceLocator
    {
        private readonly TinyIoCContainer _container = new TinyIoCContainer();

        public void AutoRegister()
        {
            _container.AutoRegister();
        }

        public void AutoRegister(Func<Type, bool> registrationPredicate)
        {
            _container.AutoRegister(registrationPredicate);
        }

        public void AutoRegister(IEnumerable<Assembly> assemblies)
        {
            _container.AutoRegister(assemblies);
        }

        public void AutoRegister(IEnumerable<Assembly> assemblies, Func<Type, bool> registrationPredicate)
        {
            _container.AutoRegister(assemblies, registrationPredicate);
        }

        public IRegisterOptions Register(Type registerType)
        {
            return new RegisterOptions(_container.Register(registerType));
        }

        public IRegisterOptions Register(Type registerType, string name)
        {
            return new RegisterOptions(_container.Register(registerType, name));
        }

        public IRegisterOptions Register(Type registerType, Type registerImplementation)
        {
            return new RegisterOptions(_container.Register(registerType, registerImplementation));
        }

        public IRegisterOptions Register(Type registerType, Type registerImplementation, string name)
        {
            return new RegisterOptions(_container.Register(registerType, registerImplementation, name));
        }

        public IRegisterOptions Register(Type registerType, object instance)
        {
            return new RegisterOptions(_container.Register(registerType, instance));
        }

        public IRegisterOptions Register(Type registerType, object instance, string name)
        {
            return new RegisterOptions(_container.Register(registerType, instance, name));
        }

        public IRegisterOptions Register(Type registerType, Type registerImplementation, object instance)
        {
            return new RegisterOptions(_container.Register(registerType, registerImplementation, instance));
        }

        public IRegisterOptions Register(Type registerType, Type registerImplementation, object instance,
            string name)
        {
            return new RegisterOptions(_container.Register(registerType, registerImplementation, instance, name));
        }

        public IRegisterOptions Register(Type registerType, Func<object> factory)
        {
            return new RegisterOptions(_container.Register(registerType, (conctainer, overloads) => factory()));
        }

        public IRegisterOptions Register(Type registerType, Func<object> factory, string name)
        {
            return new RegisterOptions(_container.Register(registerType, (conctainer, overloads) => factory(), name));
        }

        public IRegisterOptions Register<RegisterType>()
            where RegisterType : class
        {
            return new RegisterOptions(_container.Register<RegisterType>());
        }

        public IRegisterOptions Register<RegisterType>(Func<RegisterType> factory)
            where RegisterType : class
        {
            return new RegisterOptions(_container.Register((conctainer, overloads) => factory()));
        }

        public IRegisterOptions Register<RegisterType>(Func<RegisterType> factory, string name)
            where RegisterType : class
        {
            return new RegisterOptions(_container.Register((conctainer, overloads) => factory(), name));
        }


        public IRegisterOptions Register<RegisterType>(string name)
            where RegisterType : class
        {
            return new RegisterOptions(_container.Register<RegisterType>(name));
        }

        public IRegisterOptions Register<RegisterType, RegisterImplementation>()
            where RegisterType : class
            where RegisterImplementation : class, RegisterType
        {
            return new RegisterOptions(_container.Register<RegisterType, RegisterImplementation>());
        }

        public IRegisterOptions Register<RegisterType, RegisterImplementation>(string name)
            where RegisterType : class
            where RegisterImplementation : class, RegisterType
        {
            return new RegisterOptions(_container.Register<RegisterType, RegisterImplementation>(name));
        }

        public IRegisterOptions Register<RegisterType>(RegisterType instance)
            where RegisterType : class
        {
            return new RegisterOptions(_container.Register(instance));
        }

        public IRegisterOptions Register<RegisterType>(RegisterType instance, string name)
            where RegisterType : class
        {
            return new RegisterOptions(_container.Register(instance, name));
        }

        public IRegisterOptions Register<RegisterType, RegisterImplementation>(RegisterImplementation instance)
            where RegisterType : class
            where RegisterImplementation : class, RegisterType
        {
            return new RegisterOptions(_container.Register<RegisterType, RegisterImplementation>(instance));
        }

        public IRegisterOptions Register<RegisterType, RegisterImplementation>(RegisterImplementation instance,
            string name)
            where RegisterType : class
            where RegisterImplementation : class, RegisterType
        {
            return new RegisterOptions(_container.Register<RegisterType, RegisterImplementation>(instance, name));
        }

        public IMultiRegisterOptions RegisterMultiple<RegisterType>(IEnumerable<Type> implementationTypes)
        {
            return new MultiRegisterOptions(_container.RegisterMultiple<RegisterType>(implementationTypes));
        }

        public IMultiRegisterOptions RegisterMultiple(Type registrationType,
            IEnumerable<Type> implementationTypes)
        {
            return new MultiRegisterOptions(_container.RegisterMultiple(registrationType, implementationTypes));
        }

        public object Resolve(Type resolveType)
        {
            return _container.Resolve(resolveType);
        }

        public object Resolve(Type resolveType, string name)
        {
            return _container.Resolve(resolveType, name);
        }

        public ResolveType Resolve<ResolveType>() where ResolveType : class
        {
            return _container.Resolve<ResolveType>();
        }

        public ResolveType Resolve<ResolveType>(string name)
            where ResolveType : class
        {
            return _container.Resolve<ResolveType>(name);
        }

        public object GetService(Type serviceType)
        {
            return Resolve(serviceType);
        }
    }
}