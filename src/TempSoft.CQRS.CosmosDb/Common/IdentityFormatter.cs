using System;

namespace TempSoft.CQRS.CosmosDb.Common
{
    public static class IdentityFormatter
    {
        public static string Format(string entityId, string entityType)
        {
            var entityIdSafe = Base64EncodeSafe(entityId);
            return $"{entityType}_{entityIdSafe}";
        }


        private static string Base64EncodeSafe(string entityId)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(entityId);
            return Convert.ToBase64String(plainTextBytes).Replace("/", "[slash]");
        }
    }
}