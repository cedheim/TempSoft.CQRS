using System;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.Demo.Domain.Theatres.Events
{
    public class SlotAdded : EventBase
    {
        private SlotAdded() { }

        public SlotAdded(Guid slotId, string name, int order)
        {
            SlotId = slotId;
            Name = name;
            Order = order;
        }

        public Guid SlotId { get; private set; }
        public string Name { get; private set; }
        public int Order { get; private set; }
    }
}