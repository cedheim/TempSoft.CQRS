using System;

namespace TempSoft.CQRS.Demo.Domain.Theatres.Models
{
    public class AuditoriumReadModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool Is3D { get; set; }

        public bool IsIMAX { get; set; }

        public bool IsTHX { get; set; }
    }
}