﻿using System;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.Demo.Domain.Theatres.Events
{
    public class AuditoriumAdded : EventBase
    {
        public AuditoriumAdded(Guid auditoriumId, string name)
        {
            AuditoriumId = auditoriumId;
            Name = name;
        }
        public Guid AuditoriumId { get; private set; }
        public string Name { get; private set; }
    }
}