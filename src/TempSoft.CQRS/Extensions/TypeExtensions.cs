using System;
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
        internal static IEnumerable<MethodInfo> GetMethodsForAttribute<TAttribute>(this Type type) where TAttribute : Attribute
        {
            while (type != default(Type))
            {
                foreach (var methodInfo in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(m => m.CustomAttributes.Any(a => a.AttributeType == typeof(TAttribute))))
                {
                    yield return methodInfo;
                }

                type = type.BaseType;
            }
        }

        internal static Action<TDomain, TArgument> GenerateHandler<TDomain, TArgument>(this MethodInfo method, Type objectType)
        {
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
                var parameterExpressions = new List<Expression>();
                var publicProperties = objectType.GetProperties();

                foreach (var parameter in parameters)
                {
                    var matchingProperty = publicProperties.FirstOrDefault(property => string.Compare(property.Name, parameter.Name, StringComparison.InvariantCultureIgnoreCase) == 0 && parameter.ParameterType.IsAssignableFrom(property.PropertyType));
                    if (matchingProperty == default(PropertyInfo))
                    {
                        throw new System.Exception($"Unable to find a property on {objectType.Name} that matches the parameter name {parameter.Name}");
                    }

                    parameterExpressions.Add(Expression.Property(eventVariable, matchingProperty));
                }

                rootCall = Expression.Call(rootParameter, method, parameterExpressions);
            }

            // generate lambda expression.
            var body = Expression.Block(new ParameterExpression[] { eventVariable }, new Expression[] { assignment, rootCall });
            var lambda = Expression.Lambda<Action<TDomain, TArgument>>(body, rootParameter, objectParameter);

            var action = lambda.Compile();

            return action;
        }

        internal static Func<TDomain, TArgument, CancellationToken, Task> GenerateAsyncHandler<TDomain, TArgument>(this MethodInfo method, Type objectType)
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
            var parameterExpressions = new List<Expression>();
            var publicProperties = objectType.GetProperties();

            foreach (var parameter in parameters)
            {
                if (parameter.ParameterType == objectType)
                {
                    parameterExpressions.Add(eventVariable);
                    continue;
                }

                if (parameter.ParameterType == typeof(CancellationToken))
                {
                    parameterExpressions.Add(cancellationParameter);
                    continue;
                }

                var matchingProperty = publicProperties.FirstOrDefault(property => string.Compare(property.Name, parameter.Name, StringComparison.InvariantCultureIgnoreCase) == 0 && parameter.ParameterType.IsAssignableFrom(property.PropertyType));
                if (matchingProperty == default(PropertyInfo))
                {
                    throw new System.Exception($"Unable to find a property on {objectType.Name} that matches the parameter name {parameter.Name}");
                }

                parameterExpressions.Add(Expression.Property(eventVariable, matchingProperty));
            }

            var rootCall = Expression.Call(rootParameter, method, parameterExpressions);

            var returnLabel = Expression.Label(typeof(Task));
            var returnValue = Expression.Return(returnLabel, rootCall, typeof(Task));
            var returnLabelExpression = Expression.Label(returnLabel, Expression.Constant(default(Task), typeof(Task)));

            // generate lambda expression.
            var body = Expression.Block(new ParameterExpression[] { eventVariable }, new Expression[] { assignment, returnValue, returnLabelExpression });
            var lambda = Expression.Lambda<Func<TDomain, TArgument, CancellationToken, Task>>(body, rootParameter, objectParameter, cancellationParameter);

            var action = lambda.Compile();

            return action;
        }
    }
}
