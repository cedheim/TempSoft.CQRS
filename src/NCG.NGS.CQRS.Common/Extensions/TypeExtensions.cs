using System;

namespace NCG.NGS.CQRS.Common.Extensions
{
    public static class TypeExtensions
    {
        public static string ToFriendlyName(this Type type)
        {
            return $"{type.FullName}, {type.Assembly.GetName().Name}";
        }
    }
}
