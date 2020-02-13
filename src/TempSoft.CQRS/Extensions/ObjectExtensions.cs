using System;
using System.Reflection;

namespace TempSoft.CQRS.Extensions
{
    public static class ObjectExtensions
    {
        public static void SetProperty(this object obj, string propertyName, object value)
        {
            var type = obj.GetType();
            var property = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (!object.ReferenceEquals(property, default(PropertyInfo)))
            {
                var method = property.SetMethod;
                if (object.ReferenceEquals(method, default(MethodInfo)))
                {
                    throw new ArgumentException($"Property {propertyName} does not have a setter");
                }

                method.Invoke(obj, new[] { value });
                return;
            }

            var field = type.GetField(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if(!object.ReferenceEquals(field, default(FieldInfo)))
            {
                field.SetValue(obj, value);
                return;
            }

            throw new ArgumentException("Unable to locate property", nameof(propertyName));
        }

        public static object GetProperty(this object obj, string propertyName)
        {
            var type = obj.GetType();
            var property = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (!object.ReferenceEquals(property, default(PropertyInfo)))
            {
                var method = property.GetMethod;
                if (object.ReferenceEquals(method, default(MethodInfo)))
                {
                    throw new ArgumentException($"Property {propertyName} does not have a getter");
                }

                return method.Invoke(obj, new object[0]);
                
            }

            var field = type.GetField(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (!object.ReferenceEquals(field, default(FieldInfo)))
            {
                return field.GetValue(obj);
            }

            throw new ArgumentException("Unable to locate property", nameof(propertyName));
        }
    }
}