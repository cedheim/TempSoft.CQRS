using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.CosmosDb.Events
{
    public class EventStreamState
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        
        public int EventCount { get; set; }

        public EventStreamStatus Status { get; set; }

        public static explicit operator EventStreamState(Document document)
        {
            var state = new EventStreamState
            {
                Id = document.Id,
                EventCount = document.GetPropertyValue<int>(nameof(EventCount)),
                Status = document.GetPropertyValue<EventStreamStatus>(nameof(Status))
            };

            return state;
        }
    }
}