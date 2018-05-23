using System;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TempSoft.CQRS.Common.Extensions;

namespace TempSoft.CQRS.CosmosDb.Queries
{
    public class QueryModelPayloadWrapper
    {
        [JsonConstructor]
        private QueryModelPayloadWrapper()
        {
        }

        public QueryModelPayloadWrapper(string id, object o)
        {
            Id = id;
            Payload = JObject.FromObject(o);
            PayloadType = o.GetType().ToFriendlyName();
        }

        [JsonProperty("id")] public string Id { get; set; }

        public JObject Payload { get; set; }

        public string PayloadType { get; set; }

        public T GetPayload<T>()
        {
            var type = Type.GetType(PayloadType);
            return (T) Payload.ToObject(type);
        }

        public static explicit operator QueryModelPayloadWrapper(Document document)
        {
            var wrapper = new QueryModelPayloadWrapper
            {
                Id = document.Id,
                Payload = document.GetPropertyValue<JObject>(nameof(Payload)),
                PayloadType = document.GetPropertyValue<string>(nameof(PayloadType))
            };

            return wrapper;
        }
    }
}