using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Demo.Domain.Theatres.Commands;
using TempSoft.CQRS.Demo.Domain.Theatres.Events;
using TempSoft.CQRS.Demo.Domain.Theatres.Exceptions;
using TempSoft.CQRS.Demo.Domain.Theatres.Models;
using TempSoft.CQRS.Domain;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.Infrastructure;

namespace TempSoft.CQRS.Demo.Domain.Theatres.Entities
{
    public class Theatre : AggregateRoot<Theatre>, IAggregateRootWithReadModel
    {
        private readonly Dictionary<Guid, Auditorium> _auditoriums;

        public Theatre()
        {
            _auditoriums = new Dictionary<Guid, Auditorium>();
        }

        public IEnumerable<Auditorium> Auditoriums => _auditoriums.Values;

        public string Name { get; private set; }

        [CommandHandler(typeof(InitializeTheatre))]
        public void Initialize(Guid aggregateRootId, string name)
        {
            ApplyChange(new TheatreInitialized(aggregateRootId, name));
        }

        [EventHandler(typeof(TheatreInitialized))]
        private void Apply(TheatreInitialized @event)
        {
            Name = @event.Name;
            Id = @event.AggregateRootId;
        }

        [CommandHandler(typeof(AddAuditorium))]
        public void AddAuditorium(Guid auditoriumId, string name)
        {
            if (_auditoriums.ContainsKey(auditoriumId))
            {
                throw new DuplicateAuditoriumAddedException($"Tried to add an auditorium with id {auditoriumId} which already exists.");
            }

            ApplyChange(new AuditoriumAdded(auditoriumId, name));
        }

        [EventHandler(typeof(AuditoriumAdded))]
        private void Apply(AuditoriumAdded @event)
        {
            _auditoriums.Add(@event.AuditoriumId, new Auditorium(this, @event.AuditoriumId, @event.Name));
        }


        public IAggregateRootReadModel GetReadModel()
        {
            return new TheatreReadModel
            {
                Id = Id,
                Version = Version,
                Name = Name,
                Auditoriums = Auditoriums.Select(a => new AuditoriumReadModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    IsIMAX = a.IsIMAX,
                    IsTHX = a.IsTHX,
                    Is3D = a.Is3D
                }).ToArray()
            };
        }
    }
}