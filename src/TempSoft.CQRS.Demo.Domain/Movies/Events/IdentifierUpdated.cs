using System;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.Demo.Domain.Movies.Events
{
    public class IdentifierUpdated : EntityEventBase
    {

        public IdentifierUpdated(string value)
        {
            Value = value;
        }

        public string Value { get; private set; }
    }
}