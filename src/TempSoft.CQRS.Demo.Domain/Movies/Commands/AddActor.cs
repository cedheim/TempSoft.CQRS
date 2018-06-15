using System;
using Newtonsoft.Json;
using TempSoft.CQRS.Commands;

namespace TempSoft.CQRS.Demo.Domain.Movies.Commands
{
    public class AddActor : CommandBase
    {
        public Guid PersonId { get; private set; }

        public AddActor(string personId)
        {
            PersonId = Guid.Parse(personId);
        }

        [JsonConstructor]
        public AddActor(Guid personId)
        {
            PersonId = personId;
        }
    }
}