using System;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Demo.Domain.Movies.Enums;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.Demo.Domain.Movies.Events
{
    public class RemovedMovieProperty : EntityEventBase
    {
        private RemovedMovieProperty() { }

        public RemovedMovieProperty(Guid entityId, MovieProperties movieProperty)
        {
            MovieProperty = movieProperty;
            EntityId = entityId;
        }

        public MovieProperties MovieProperty { get; private set; }
    }
}