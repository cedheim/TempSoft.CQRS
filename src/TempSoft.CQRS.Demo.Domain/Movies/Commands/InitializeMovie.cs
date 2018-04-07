using System;
using TempSoft.CQRS.Commands;

namespace TempSoft.CQRS.Demo.Domain.Movies.Commands
{
    public class InitializeMovie : CommandBase
    {
        private InitializeMovie() { }

        public InitializeMovie(Guid aggregateRootId, string publicId, string title)
        {
            AggregateRootId = aggregateRootId;
            PublicId = publicId;
            Title = title;
        }

        public Guid AggregateRootId { get; private set; }
        public string PublicId { get; private set; }
        public string Title { get; private set; }
    }
}