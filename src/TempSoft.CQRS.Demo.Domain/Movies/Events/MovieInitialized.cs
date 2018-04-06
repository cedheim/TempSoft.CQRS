using System;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.Demo.Domain.Movies.Events
{
    public class MovieInitialized : EventBase, IInitializationEvent
    {
        private MovieInitialized() { }

        public MovieInitialized(Guid aggregateRootId, string publicId)
        {
            PublicId = publicId;
            AggregateRootId = aggregateRootId;
        }

        public string PublicId { get; private set; }
    }
}