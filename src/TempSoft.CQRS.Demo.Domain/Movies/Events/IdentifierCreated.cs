using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.Demo.Domain.Movies.Events
{
    public class IdentifierCreated : EventBase
    {
        public IdentifierCreated(string identifierId)
        {
            IdentifierId = identifierId;
        }

        public string IdentifierId { get; private set; }
    }
}