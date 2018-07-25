using System;
using TempSoft.CQRS.Projectors;

namespace TempSoft.CQRS.Demo.Projectors.MovieLists.Models
{
    public class MovieListEntry : IProjection
    {
        public string Id { get; set; }

        public string ProjectorId { get; set; }

        public string OriginalTitle { get; set; }

        public Guid AggregateRootId { get; set; }

        public DateTime Timestamp { get; set; }
    }
}