using System;
using TempSoft.CQRS.Commands;

namespace TempSoft.CQRS.Demo.Domain.Theatres.Commands
{
    public class InitializeTheatre : CommandBase
    {
        private InitializeTheatre()
        {
        }

        public InitializeTheatre(Guid aggregateRootId, string name)
        {
            AggregateRootId = aggregateRootId;
            Name = name;
        }

        public Guid AggregateRootId { get; }

        public string Name { get; }
    }
}