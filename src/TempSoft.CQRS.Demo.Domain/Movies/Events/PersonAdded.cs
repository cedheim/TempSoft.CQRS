using System;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.Demo.Domain.Movies.Events
{
    public class PersonAdded : EventBase
    {
        public Guid PersonId { get; private set; }
        public string Role { get; private set; }

        public PersonAdded(Guid personId, string role)
        {
            PersonId = personId;
            Role = role;
        }
    }
}