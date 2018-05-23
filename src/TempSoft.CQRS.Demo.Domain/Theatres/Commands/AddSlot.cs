using System;
using TempSoft.CQRS.Commands;

namespace TempSoft.CQRS.Demo.Domain.Theatres.Commands
{
    public class AddSlot : CommandBase
    {
        private AddSlot()
        {
        }

        public AddSlot(Guid slotId, string name, int order)
        {
            SlotId = slotId;
            Name = name;
            Order = order;
        }

        public Guid SlotId { get; }
        public string Name { get; }
        public int Order { get; }
    }
}