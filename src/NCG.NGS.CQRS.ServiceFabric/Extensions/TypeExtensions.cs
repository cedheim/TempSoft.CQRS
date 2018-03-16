using System;
using System.Collections.Generic;
using System.Text;

namespace NCG.NGS.CQRS.ServiceFabric.Extensions
{
    public static class TypeExtensions
    {
        public static string ToFriendlyName(this Type type)
        {
            return $"{type.FullName}, {type.Assembly.GetName().Name}";
        }
    }
}
