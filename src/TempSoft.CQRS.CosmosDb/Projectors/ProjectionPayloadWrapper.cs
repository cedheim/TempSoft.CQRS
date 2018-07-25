using System;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TempSoft.CQRS.Common.Extensions;
using TempSoft.CQRS.Projectors;

namespace TempSoft.CQRS.CosmosDb.Projectors
{
    public class ProjectionPayloadWrapper
    {
        [JsonConstructor]
        private ProjectionPayloadWrapper()
        {
        }

        public ProjectionPayloadWrapper(IProjection projection)
        {
            Id = projection.Id;
            PayloadType = projection.GetType().ToFriendlyName();
            ProjectorId = projection.ProjectorId;
            Payload = JObject.FromObject(projection);
        }

        [JsonProperty("id")]
        public string Id { get; set; }

        public JObject Payload { get; set; }

        public string PayloadType { get; set; }

        public string ProjectorId { get; set; }

        public IProjection GetProjection()
        {
            var type = Type.GetType(PayloadType);

            return (IProjection)Payload.ToObject(type);
        }

        public static explicit operator ProjectionPayloadWrapper(Document document)
        {
            var wrapper = new ProjectionPayloadWrapper
            {
                Id = document.Id,
                Payload = document.GetPropertyValue<JObject>(nameof(Payload)),
                PayloadType = document.GetPropertyValue<string>(nameof(PayloadType)),
                ProjectorId = document.GetPropertyValue<string>(nameof(ProjectorId))
            };

            return wrapper;
        }

        [JsonProperty("_ts")]
        public long Epoch { get; set; }

        [JsonIgnore]
        public DateTime Timestamp => Epoch.ToDateTime();
    }
}