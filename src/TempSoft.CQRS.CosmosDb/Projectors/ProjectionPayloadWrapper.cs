using System;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TempSoft.CQRS.CosmosDb.Common;
using TempSoft.CQRS.CosmosDb.Extensions;
using TempSoft.CQRS.Projectors;
using TempSoft.CQRS.Extensions;

namespace TempSoft.CQRS.CosmosDb.Projectors
{
    public class ProjectionPayloadWrapper : StorageBase
    {
        public const string DocumentTypeName = "projection";

        [JsonConstructor]
        private ProjectionPayloadWrapper()
        {
        }

        public ProjectionPayloadWrapper(IProjection projection)
            : base(projection.ProjectorId, DocumentTypeName)
        {
            Id = CreateIdentifier(projection.Id);
            PayloadType = projection.GetType().ToFriendlyName();
            ProjectionId = projection.Id;
            Payload = JObject.FromObject(projection);
        }

        public string ProjectionId { get; set; }

        public JObject Payload { get; set; }

        public string PayloadType { get; set; }
        
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
                ProjectionId = document.GetPropertyValue<string>(nameof(ProjectionId)),
                Payload = document.GetPropertyValue<JObject>(nameof(Payload)),
                PayloadType = document.GetPropertyValue<string>(nameof(PayloadType)),

                PartitionId = document.GetPropertyValue<string>(nameof(PartitionId)),
                DocumentType = document.GetPropertyValue<string>(nameof(DocumentType)),
                Epoch = document.Timestamp.ToUnixTime(),
            };

            return wrapper;
        }
        
        public static string CreateIdentifier(string projectionId)
        {
            return IdentityFormatter.Format(projectionId, DocumentTypeName);
        }
    }
}