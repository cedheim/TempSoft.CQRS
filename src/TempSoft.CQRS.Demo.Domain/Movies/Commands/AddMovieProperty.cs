using System;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Demo.Domain.Movies.Enums;

namespace TempSoft.CQRS.Demo.Domain.Movies.Commands
{
    public class AddMovieProperty : EntityCommandBase
    {
        private AddMovieProperty()
        {
        }

        public AddMovieProperty(Guid entityId, MovieProperties movieProperty)
        {
            MovieProperty = movieProperty;
            EntityId = entityId;
        }

        public MovieProperties MovieProperty { get; }
    }
}