﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace TempSoft.CQRS.Extensions
{
    public static class TypeExtensions
    {
        private static readonly MethodInfo TaskRunMethod;
        private static readonly MethodInfo TaskRunMethodWithReturnType;

        static TypeExtensions()
        {
            TaskRunMethod = typeof(Task).GetMethods()
                .Where(m => m.Name == "Run")
                .Select(m => new {Method = m, Parameters = m.GetParameters()})
                .FirstOrDefault(q => q.Parameters.Length == 1 && q.Parameters[0].ParameterType == typeof(Action))
                .Method;
            TaskRunMethodWithReturnType = typeof(Task).GetMethods()
                .Where(m => m.Name == "Run" && m.IsGenericMethodDefinition)
                .Select(m => new { Method = m, Parameters = m.GetParameters() })
                .FirstOrDefault(q => q.Parameters.Length == 1 && q.Parameters[0].ParameterType.IsGenericType && q.Parameters[0].ParameterType.GetGenericTypeDefinition() == typeof(Func<>))
                .Method;
        }

        internal static IEnumerable<MethodInfo> GetMethodsForAttribute<TAttribute>(this Type type)
            where TAttribute : Attribute
        {
            while (type != default(Type))
            {
                foreach (var methodInfo in type
                    .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(m => m.CustomAttributes.Any(a => a.AttributeType == typeof(TAttribute))))
                    yield return methodInfo;

                type = type.BaseType;
            }
        }

        internal static Action<TDomain, TArgument> GenerateHandler<TDomain, TArgument>(this MethodInfo method)
        {
            return GenerateHandler<TDomain, TArgument>(method, typeof(TArgument));
        }

        internal static Action<TDomain, TArgument> GenerateHandler<TDomain, TArgument>(this MethodInfo method,
            Type objectType)
        {
            if (!typeof(TArgument).IsAssignableFrom(objectType))
            {
                throw new Exception($"Unable to assign value of type {objectType.Name} to {typeof(TArgument).Name}.");
            }


            // declaration of action parameters.
            var rootParameter = Expression.Parameter(typeof(TDomain), "root");
            var objectParameter = Expression.Parameter(typeof(TArgument), "o");

            // cast the input event to the actual object type.
            var castParameterToObjectType = Expression.TypeAs(objectParameter, objectType);
            var eventVariable = Expression.Parameter(objectType, "e");
            var assignment = Expression.Assign(eventVariable, castParameterToObjectType);

            // generate call to root method.
            MethodCallExpression rootCall;
            var parameters = method.GetParameters();

            // use explicit call.
            if (parameters.Length == 1 && parameters[0].ParameterType == objectType)
            {
                rootCall = Expression.Call(rootParameter, method, eventVariable);
            }
            // use implicit call.
            else
            {
                var publicProperties = objectType.GetProperties();
                var parameterExpressions = MapParametersToInput(parameters, publicProperties, eventVariable, objectType);

                rootCall = Expression.Call(rootParameter, method, parameterExpressions);
            }

            // generate lambda expression.
            var body = Expression.Block(new[] {eventVariable}, assignment, rootCall);
            var lambda = Expression.Lambda<Action<TDomain, TArgument>>(body, rootParameter, objectParameter);

            var action = lambda.Compile();

            return action;
        }

        internal static Func<TDomain, TArgument, CancellationToken, Task> GenerateAsyncHandler<TDomain, TArgument>(this MethodInfo method)
        {
            return GenerateAsyncHandler<TDomain, TArgument>(method, typeof(TArgument));
        }

        internal static Func<TDomain, TArgument, CancellationToken, Task> GenerateAsyncHandler<TDomain, TArgument>(
            this MethodInfo method, Type objectType)
        {
            if (!typeof(TArgument).IsAssignableFrom(objectType))
            {
                throw new Exception($"Unable to assign value of type {objectType.Name} to {typeof(TArgument).Name}.");
            }

            // declaration of action parameters.
            var rootParameter = Expression.Parameter(typeof(TDomain), "root");
            var objectParameter = Expression.Parameter(typeof(TArgument), "o");
            var cancellationParameter = Expression.Parameter(typeof(CancellationToken), "cancellationToken");

            // cast the input event to the actual object type.
            var castParameterToObjectType = Expression.TypeAs(objectParameter, objectType);
            var eventVariable = Expression.Parameter(objectType, "e");
            var assignment = Expression.Assign(eventVariable, castParameterToObjectType);

            // generate call to root method.
            var parameters = method.GetParameters();

            // use explicit call.
            var publicProperties = objectType.GetProperties();
            var parameterExpressions = MapParametersToInput(parameters, publicProperties, eventVariable, objectType, cancellationParameter);


            var rootCall = Expression.Call(rootParameter, method, parameterExpressions);
            // if the method is void 
            if (method.ReturnType == typeof(void))
            {
                var callRootLambda = Expression.Lambda<Action>(rootCall);
                rootCall = Expression.Call(TaskRunMethod, callRootLambda);
            }

            var returnLabel = Expression.Label(typeof(Task));
            var returnValue = Expression.Return(returnLabel, rootCall, typeof(Task));
            var returnLabelExpression = Expression.Label(returnLabel, Expression.Constant(default(Task), typeof(Task)));

            // generate lambda expression.
            var body = Expression.Block(new[] {eventVariable}, assignment, returnValue, returnLabelExpression);
            var lambda =
                Expression.Lambda<Func<TDomain, TArgument, CancellationToken, Task>>(body, rootParameter,
                    objectParameter, cancellationParameter);

            var action = lambda.Compile();

            return action;
        }

        private static IEnumerable<Expression> MapParametersToInput(ParameterInfo[] parameters, PropertyInfo[] publicProperties, Expression eventVariable, Type objectType, Expression cancellationParameter = default(Expression))
        {
            
            foreach (var parameter in parameters)
            {
                if (parameter.ParameterType == objectType)
                {
                    yield return eventVariable;
                    continue;
                }

                if (parameter.ParameterType == typeof(CancellationToken) && !object.ReferenceEquals(cancellationParameter, default(Expression)))
                {
                    yield return cancellationParameter;
                    continue;
                }

                var matchingProperty = publicProperties.FirstOrDefault(property =>
                    string.Compare(property.Name, parameter.Name, StringComparison.InvariantCultureIgnoreCase) == 0 &&
                    parameter.ParameterType.IsAssignableFrom(property.PropertyType));
                if (matchingProperty == default(PropertyInfo))
                    throw new Exception($"Unable to find a property on {objectType.Name} that matches the parameter name {parameter.Name}");

                yield return Expression.Property(eventVariable, matchingProperty);
            }
        }

        internal static Func<TDomain, TArgument, CancellationToken, Task<TReturn>> GenerateAsyncHandlerWithReturnType<TDomain, TArgument, TReturn>(this MethodInfo method)
        {
            return GenerateAsyncHandlerWithReturnType<TDomain, TArgument, TReturn>(method, typeof(TArgument));
        }

        internal static Func<TDomain, TArgument, CancellationToken, Task<TReturn>> GenerateAsyncHandlerWithReturnType<TDomain, TArgument, TReturn>(this MethodInfo method, Type objectType)
        {
            // declaration of action parameters.
            var rootParameter = Expression.Parameter(typeof(TDomain), "root");
            var objectParameter = Expression.Parameter(typeof(TArgument), "o");
            var cancellationParameter = Expression.Parameter(typeof(CancellationToken), "cancellationToken");

            // cast the input event to the actual object type.
            var castParameterToObjectType = Expression.TypeAs(objectParameter, objectType);
            var eventVariable = Expression.Parameter(objectType, "e");
            var assignment = Expression.Assign(eventVariable, castParameterToObjectType);

            // generate call to root method.
            var parameters = method.GetParameters();

            // use explicit call.
            var publicProperties = objectType.GetProperties();
            var parameterExpressions = MapParametersToInput(parameters, publicProperties, eventVariable, objectType, cancellationParameter);


            var rootCall = Expression.Call(rootParameter, method, parameterExpressions);

            // if the method is non async, create an async wrapper.
            if (method.ReturnType == typeof(TReturn))
            {
                var taskRunMethod = TaskRunMethodWithReturnType.MakeGenericMethod(typeof(TReturn));

                var callRootLambda = Expression.Lambda<Func<TReturn>>(rootCall);
                rootCall = Expression.Call(taskRunMethod, callRootLambda);
            }
            else if (method.ReturnType != typeof(Task<TReturn>))
            {
                throw new ArgumentException($"Unable to create wrapper for method it has return type ${method.ReturnType.Name} but {typeof(Task<TReturn>).Name} is required.");
            }

            var returnLabel = Expression.Label(typeof(Task<TReturn>));
            var returnValue = Expression.Return(returnLabel, rootCall, typeof(Task<TReturn>));
            var returnLabelExpression = Expression.Label(returnLabel, Expression.Constant(default(Task<TReturn>), typeof(Task<TReturn>)));

            // generate lambda expression.
            var body = Expression.Block(new[] { eventVariable }, assignment, returnValue, returnLabelExpression);
            var lambda =
                Expression.Lambda<Func<TDomain, TArgument, CancellationToken, Task<TReturn>>>(body, rootParameter,
                    objectParameter, cancellationParameter);

            var action = lambda.Compile();

            return action;
        }

        public static string ToFriendlyName(this Type type)
        {
            return $"{type.FullName}, {type.Assembly.GetName().Name}";
        }
    }
}