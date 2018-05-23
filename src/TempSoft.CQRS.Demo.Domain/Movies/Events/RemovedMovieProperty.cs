using System;
using TempSoft.CQRS.Demo.Domain.Movies.Enums;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.Demo.Domain.Movies.Events
{
    public class AddedMovieProperty : EntityEventBase
    {
        private AddedMovieProperty()
        {
        }

        public AddedMovieProperty(Guid entityId, MovieProperties movieProperty)
        {
            MovieProperty = movieProperty;
            EntityId = entityId;
        }

        public MovieProperties MovieProperty { get; }
    }
}