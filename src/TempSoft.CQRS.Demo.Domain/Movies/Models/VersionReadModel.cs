using System;

namespace TempSoft.CQRS.Demo.Domain.Movies.Models
{
    public class VersionReadModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool Has3D { get; set; }

        public bool HasIMAX { get; set; }

        public bool HasTHX { get; set; }
    }
}