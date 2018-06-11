using System;
using TempSoft.CQRS.Domain;

namespace TempSoft.CQRS.Demo.Domain.Movies.Models
{
    public class MovieModel : IAggregateRootReadModel
    {
        public string OriginalTitle { get; set; }
        public LocalInformationModel[] LocalInformation { get; set; }
        public Guid Id { get; set; }
        public int Version { get; set; }
    }
}