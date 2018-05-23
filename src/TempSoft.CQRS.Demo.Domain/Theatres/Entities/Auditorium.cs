using System;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Demo.Domain.Theatres.Commands;
using TempSoft.CQRS.Demo.Domain.Theatres.Enums;
using TempSoft.CQRS.Demo.Domain.Theatres.Events;
using TempSoft.CQRS.Demo.Domain.Theatres.Exceptions;
using TempSoft.CQRS.Domain;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.Demo.Domain.Theatres.Entities
{
    public class Auditorium : AggregateRoot<Theatre>.Entity<Auditorium>
    {
        public Auditorium(Theatre root, Guid id, string name)
            : base(root, id)
        {
            Name = name;
        }

        public string Name { get; }

        public bool Is3D { get; private set; }

        public bool IsIMAX { get; private set; }

        public bool IsTHX { get; private set; }

        [CommandHandler(typeof(AddAuditoriumProperty))]
        public void AddAuditoriumProperty(AuditoriumProperties auditoriumProperty)
        {
            switch (auditoriumProperty)
            {
                case AuditoriumProperties.Is3D:
                    if (Is3D)
                        throw new AuditoriumPropertyException($"Auditorium already has property {auditoriumProperty}");
                    break;
                case AuditoriumProperties.IsIMAX:
                    if (IsIMAX)
                        throw new AuditoriumPropertyException($"Auditorium already has property {auditoriumProperty}");
                    break;
                case AuditoriumProperties.IsTHX:
                    if (IsTHX)
                        throw new AuditoriumPropertyException($"Auditorium already has property {auditoriumProperty}");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Root.ApplyChange(new AuditoriumPropertyAdded(Id, auditoriumProperty));
        }

        [CommandHandler(typeof(RemoveAuditoriumProperty))]
        public void RemoveAuditoriumProperty(AuditoriumProperties auditoriumProperty)
        {
            switch (auditoriumProperty)
            {
                case AuditoriumProperties.Is3D:
                    if (!Is3D)
                        throw new AuditoriumPropertyException($"Auditorium already has property {auditoriumProperty}");
                    break;
                case AuditoriumProperties.IsIMAX:
                    if (!IsIMAX)
                        throw new AuditoriumPropertyException($"Auditorium already has property {auditoriumProperty}");
                    break;
                case AuditoriumProperties.IsTHX:
                    if (!IsTHX)
                        throw new AuditoriumPropertyException($"Auditorium already has property {auditoriumProperty}");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Root.ApplyChange(new AuditoriumPropertyRemoved(Id, auditoriumProperty));
        }

        [EventHandler(typeof(AuditoriumPropertyAdded))]
        private void Apply(AuditoriumPropertyAdded @event)
        {
            switch (@event.AuditoriumProperty)
            {
                case AuditoriumProperties.Is3D:
                    Is3D = true;
                    break;
                case AuditoriumProperties.IsIMAX:
                    IsIMAX = true;
                    break;
                case AuditoriumProperties.IsTHX:
                    IsTHX = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [EventHandler(typeof(AuditoriumPropertyRemoved))]
        private void Apply(AuditoriumPropertyRemoved @event)
        {
            switch (@event.AuditoriumProperty)
            {
                case AuditoriumProperties.Is3D:
                    Is3D = false;
                    break;
                case AuditoriumProperties.IsIMAX:
                    IsIMAX = false;
                    break;
                case AuditoriumProperties.IsTHX:
                    IsTHX = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}