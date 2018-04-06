using System;
using TempSoft.CQRS.Commands;

namespace TempSoft.CQRS.Demo.Domain.Theatres.Commands
{
    public class AddAuditorium : CommandBase
    {
        private AddAuditorium() { }

        public AddAuditorium(Guid auditoriumId, string name)
        {
            AuditoriumId = auditoriumId;
            Name = name;
        }

        public Guid AuditoriumId { get; private set; }
        public string Name { get; private set; }
    }
}