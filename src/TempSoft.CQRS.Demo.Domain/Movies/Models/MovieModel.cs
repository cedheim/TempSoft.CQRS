using System;
using System.Collections.Generic;
using TempSoft.CQRS.Domain;

namespace TempSoft.CQRS.Demo.Domain.Movies.Models
{
    public class MovieModel : IAggregateRootReadModel
    {
        public string OriginalTitle { get; set; }
        public Dictionary<string, LocalInformationModel> LocalInformation { get; set; }
        public Dictionary<string, string> Identifiers { get; set; }
        public Dictionary<string, Guid[]> Persons { get; set; }
        public Guid Id { get; set; }
        public int Version { get; set; }
    }
}