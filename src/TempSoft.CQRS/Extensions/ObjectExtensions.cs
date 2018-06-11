using System;
using System.Reflection;

namespace TempSoft.CQRS.Extensions
{
    public static class ObjectExtensions
    {
        public static void SetProperty(this object obj, string propertyName, object value)
        {
            var type = obj.GetType();
            var property = type.GetProperty(propertyName);
            if (object.ReferenceEquals(property, default(PropertyInfo)))
            {
                throw new ArgumentException($"Unable to find property {propertyName}");
            }

            var method = property.SetMethod;
            if (object.ReferenceEquals(method, default(MethodInfo)))
            {
                throw new ArgumentException($"Property {propertyName} does not have a setter");
            }

            method.Invoke(obj, new[] {value});
        }
    }
}