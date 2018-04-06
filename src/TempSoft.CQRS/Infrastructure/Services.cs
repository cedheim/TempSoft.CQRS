using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using TinyIoC;

namespace TempSoft.CQRS.Infrastructure
{
    public static class Services
    {
        private static readonly TinyIoCContainer _container = new TinyIoCContainer();

        public static void AutoRegister()
        {  
            _container.AutoRegister();
        }

        public static void AutoRegister(Func<Type, bool> registrationPredicate)
        {  
            _container.AutoRegister(registrationPredicate);
        }

        public static void AutoRegister(IEnumerable<Assembly> assemblies)
        {
            _container.AutoRegister(assemblies);
        }

        public static void AutoRegister(IEnumerable<Assembly> assemblies, Func<Type, bool> registrationPredicate)
        {
            _container.AutoRegister(assemblies, registrationPredicate);
        }

        public static RegisterOptions Register(Type registerType)
        {
            return new RegisterOptions(_container.Register(registerType));
        }

        public static RegisterOptions Register(Type registerType, string name)
        {
            return new RegisterOptions(_container.Register(registerType, name));

        }

        public static RegisterOptions Register(Type registerType, Type registerImplementation)
        {
            return new RegisterOptions(_container.Register(registerType, registerImplementation));
        }

        public static RegisterOptions Register(Type registerType, Type registerImplementation, string name)
        {
            return new RegisterOptions(_container.Register(registerType, registerImplementation, name));
        }

        public static RegisterOptions Register(Type registerType, object instance)
        {
            return new RegisterOptions(_container.Register(registerType, instance));
        }

        public static RegisterOptions Register(Type registerType, object instance, string name)
        {
            return new RegisterOptions(_container.Register(registerType, instance, name));
        }

        public static RegisterOptions Register(Type registerType, Type registerImplementation, object instance)
        {
            return new RegisterOptions(_container.Register(registerType, registerImplementation, instance));
        }

        public static RegisterOptions Register(Type registerType, Type registerImplementation, object instance, string name)
        {
            return new RegisterOptions(_container.Register(registerType, registerImplementation, instance, name));
        }

        public static RegisterOptions Register(Type registerType, Func<object> factory)
        {
            return new RegisterOptions(_container.Register(registerType, (conctainer, overloads) => factory()));
        }
        
        public static RegisterOptions Register(Type registerType, Func<object> factory, string name)
        {
            return new RegisterOptions(_container.Register(registerType, (conctainer, overloads) => factory(), name));
        }

        public static RegisterOptions Register<RegisterType>()
            where RegisterType : class
        {
            return new RegisterOptions(_container.Register<RegisterType>());
        }

        public static RegisterOptions Register<RegisterType>(Func<RegisterType> factory)
            where RegisterType : class
        {
            return new RegisterOptions(_container.Register<RegisterType>((conctainer, overloads) => factory()));
        }

        public static RegisterOptions Register<RegisterType>(Func<RegisterType> factory, string name)
            where RegisterType : class
        {
            return new RegisterOptions(_container.Register<RegisterType>((conctainer, overloads) => factory(), name));
        }


        public static RegisterOptions Register<RegisterType>(string name)
            where RegisterType : class
        {
            return new RegisterOptions(_container.Register<RegisterType>(name));
        }

        public static RegisterOptions Register<RegisterType, RegisterImplementation>()
            where RegisterType : class
            where RegisterImplementation : class, RegisterType
        {

            return new RegisterOptions(_container.Register<RegisterType, RegisterImplementation>());
        }

        public static RegisterOptions Register<RegisterType, RegisterImplementation>(string name)
            where RegisterType : class
            where RegisterImplementation : class, RegisterType
        {
            return new RegisterOptions(_container.Register<RegisterType, RegisterImplementation>(name));
        }

        public static RegisterOptions Register<RegisterType>(RegisterType instance)
           where RegisterType : class
        {
            return new RegisterOptions(_container.Register<RegisterType>(instance));
        }

        public static RegisterOptions Register<RegisterType>(RegisterType instance, string name)
            where RegisterType : class
        {
            return new RegisterOptions(_container.Register<RegisterType>(instance, name));
        }

        public static RegisterOptions Register<RegisterType, RegisterImplementation>(RegisterImplementation instance)
            where RegisterType : class
            where RegisterImplementation : class, RegisterType
        {
            return new RegisterOptions(_container.Register<RegisterType, RegisterImplementation>(instance));
        }

        public static RegisterOptions Register<RegisterType, RegisterImplementation>(RegisterImplementation instance, string name)
            where RegisterType : class
            where RegisterImplementation : class, RegisterType
        {

            return new RegisterOptions(_container.Register<RegisterType, RegisterImplementation>(instance, name));
        }

        public static MultiRegisterOptions RegisterMultiple<RegisterType>(IEnumerable<Type> implementationTypes)
        {
            return new MultiRegisterOptions(_container.RegisterMultiple<RegisterType>(implementationTypes));
        }

        public static MultiRegisterOptions RegisterMultiple(Type registrationType, IEnumerable<Type> implementationTypes)
        {
            return new MultiRegisterOptions(_container.RegisterMultiple(registrationType, implementationTypes));
        }

        public static object Resolve(Type resolveType)
        {
            return _container.Resolve(resolveType);
        }

        public static object Resolve(Type resolveType, string name)
        {
            return _container.Resolve(resolveType, name);
        }
        
        public static ResolveType Resolve<ResolveType>() where ResolveType : class
        {
            return _container.Resolve<ResolveType>();
        }
        
        public static ResolveType Resolve<ResolveType>(string name)
            where ResolveType : class
        {
            return _container.Resolve<ResolveType>(name);
        }
    }
}