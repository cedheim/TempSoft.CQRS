using System;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Demo.Domain.Movies.Values;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.Demo.Domain.Movies.Events
{
    public class LocalInformationCreated : EventBase
    {

        private LocalInformationCreated()
        {
        }

        public LocalInformationCreated(Guid entityId, Country country)
        {
            EntityId = entityId;
            Country = country;
        }

        public Guid EntityId { get; private set; }
        public Country Country { get; private set; }
    }
}