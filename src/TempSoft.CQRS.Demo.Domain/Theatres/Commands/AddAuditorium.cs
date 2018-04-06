using System;
using TempSoft.CQRS.Commands;

namespace TempSoft.CQRS.Demo.Domain.Theatres.Commands
{
    public class AddAuditorium : CommandBase
    {
        private AddAuditorium() { }

        public AddAuditorium(string auditoriumId, string name)
        {
            AuditoriumId = auditoriumId;
            Name = name;
        }

        public string AuditoriumId { get; private set; }
        public string Name { get; private set; }

    }
}