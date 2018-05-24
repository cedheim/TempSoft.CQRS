using System;
using System.Collections.Generic;
using System.Reflection;

namespace TempSoft.CQRS.Infrastructure
{
    public interface IServiceLocator : IServiceProvider
    {
        void AutoRegister();

        void AutoRegister(Func<Type, bool> registrationPredicate);

        void AutoRegister(IEnumerable<Assembly> assemblies);

        void AutoRegister(IEnumerable<Assembly> assemblies, Func<Type, bool> registrationPredicate);

        IRegisterOptions Register(Type registerType);

        IRegisterOptions Register(Type registerType, string name);

        IRegisterOptions Register(Type registerType, Type registerImplementation);

        IRegisterOptions Register(Type registerType, Type registerImplementation, string name);

        IRegisterOptions Register(Type registerType, object instance);

        IRegisterOptions Register(Type registerType, object instance, string name);

        IRegisterOptions Register(Type registerType, Type registerImplementation, object instance);

        IRegisterOptions Register(Type registerType, Type registerImplementation, object instance, string name);

        IRegisterOptions Register(Type registerType, Func<object> factory);

        IRegisterOptions Register(Type registerType, Func<object> factory, string name);

        IRegisterOptions Register<RegisterType>() where RegisterType : class;

        IRegisterOptions Register<RegisterType>(Func<RegisterType> factory) where RegisterType : class;

        IRegisterOptions Register<RegisterType>(Func<RegisterType> factory, string name) where RegisterType : class;

        IRegisterOptions Register<RegisterType>(string name) where RegisterType : class;

        IRegisterOptions Register<RegisterType, RegisterImplementation>() where RegisterType : class where RegisterImplementation : class, RegisterType;

        IRegisterOptions Register<RegisterType, RegisterImplementation>(string name)
            where RegisterType : class
            where RegisterImplementation : class, RegisterType;

        IRegisterOptions Register<RegisterType>(RegisterType instance)
            where RegisterType : class;

        IRegisterOptions Register<RegisterType>(RegisterType instance, string name)
            where RegisterType : class;

        IRegisterOptions Register<RegisterType, RegisterImplementation>(RegisterImplementation instance)
            where RegisterType : class
            where RegisterImplementation : class, RegisterType;

        IRegisterOptions Register<RegisterType, RegisterImplementation>(RegisterImplementation instance,
            string name)
            where RegisterType : class
            where RegisterImplementation : class, RegisterType;

        IMultiRegisterOptions RegisterMultiple<RegisterType>(IEnumerable<Type> implementationTypes);

        IMultiRegisterOptions RegisterMultiple(Type registrationType,
            IEnumerable<Type> implementationTypes);

        object Resolve(Type resolveType);

        object Resolve(Type resolveType, string name);

        ResolveType Resolve<ResolveType>() where ResolveType : class;

        ResolveType Resolve<ResolveType>(string name)
            where ResolveType : class;
    }
}