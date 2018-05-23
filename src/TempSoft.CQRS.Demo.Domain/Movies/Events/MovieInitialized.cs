using System;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.Demo.Domain.Movies.Events
{
    public class MovieInitialized : EventBase, IInitializationEvent
    {
        private MovieInitialized()
        {
        }

        public MovieInitialized(Guid aggregateRootId, string publicId, string title)
        {
            PublicId = publicId;
            AggregateRootId = aggregateRootId;
            Title = title;
        }

        public string PublicId { get; }

        public string Title { get; }
    }
}