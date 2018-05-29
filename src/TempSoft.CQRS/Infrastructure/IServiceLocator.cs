using System;
using System.Collections.Generic;
using System.Reflection;

namespace TempSoft.CQRS.Infrastructure
{
    public interface IServiceLocator : IDisposable
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
        
        bool CanResolve(Type resolveType);
        bool CanResolve<ResolveType>() where ResolveType : class;
        bool CanResolve<ResolveType>(string name) where ResolveType : class;
        bool TryResolve(Type resolveType, out object resolvedType);
        bool TryResolve(Type resolveType, string name, out object resolvedType);
        bool TryResolve<ResolveType>(out ResolveType resolvedType) where ResolveType : class;
        bool TryResolve<ResolveType>(string name, out ResolveType resolvedType) where ResolveType : class;
        IEnumerable<object> ResolveAll(Type resolveType, bool includeUnnamed);
        IEnumerable<object> ResolveAll(Type resolveType);
        IEnumerable<ResolveType> ResolveAll<ResolveType>(bool includeUnnamed) where ResolveType : class;
        IEnumerable<ResolveType> ResolveAll<ResolveType>() where ResolveType : class;
        void BuildUp(object input);
    }
}