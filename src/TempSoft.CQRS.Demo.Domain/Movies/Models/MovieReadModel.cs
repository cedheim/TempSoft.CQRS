using System;
using TempSoft.CQRS.Domain;

namespace TempSoft.CQRS.Demo.Domain.Movies.Models
{
    public class MovieReadModel : IAggregateRootReadModel
    {
        public Guid Id { get; set; }
        public int Version { get; set; }

        public VersionReadModel[] Versions { get; set; }

        public string PublicId { get; set; }

        public string Title { get; set; }

    }
}