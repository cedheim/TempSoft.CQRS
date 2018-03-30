using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;

namespace TempSoft.CQRS.Tests.Mocks
{
    public static class ReflectionUtils
    {
        private static readonly object[] NoArguments = { };
        private static readonly Type EnumerableType = typeof(IEnumerable);

        public static void SetPrivateProperty<TPropertyValue>(this object that,
            Expression<Func<TPropertyValue>> propertyExpression, TPropertyValue value)
        {
            var memberExpr = propertyExpression.Body as MemberExpression;
            if (memberExpr == null)
                throw new ArgumentException("propertyExpression should represent access to a member");
            string memberName = memberExpr.Member.Name;

            var propertyInfo = that.GetType()
                .GetProperty(memberName,
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.SetProperty);

            if (propertyInfo == null)
                throw new ArgumentException($"Cannot set value on property {memberName} on {that.GetType().Name}");

            propertyInfo.SetValue(that, value);
        }

        public static TProperty GetPropertyValue<TProperty>(this object that, string propertyName)
        {
            var type = that.GetType();

            while (type != null)
            {
                var properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                var property = properties.FirstOrDefault(p => p.Name == propertyName);

                if (property != null)
                {
                    var method = property.GetMethod;
                    return (TProperty)method.Invoke(that, new object[0]);
                }

                var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                var field = fields.FirstOrDefault(p => p.Name == propertyName);
                if (field != null)
                {
                    return (TProperty)field.GetValue(that);
                }

                type = type.BaseType;
            }

            return default(TProperty);
        }
        
        public static object CallPrivateMethod(this object that, string methodName, params object[] args)
        {
            var methodInfo = that.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (methodInfo == null) throw new ArgumentException($"Method {methodName} does not exist on {that.GetType().Name}");

            return methodInfo.Invoke(that, args);
        }

        public static T Clone<T>(this T that)
        {
            if (object.ReferenceEquals(that, default(T)))
            {
                return default(T);
            }

            var type = that.GetType();
            var serializer = new DataContractSerializer(type);
            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, that);

                stream.Seek(0, SeekOrigin.Begin);

                return (T)serializer.ReadObject(stream);
            }
        }
    }
}