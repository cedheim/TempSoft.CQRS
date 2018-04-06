using System;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.Demo.Domain.Movies.Events
{
    public class AddedMovieVersion : EventBase
    {
        private AddedMovieVersion() { }

        public AddedMovieVersion(Guid versionId)
        {
            VersionId = versionId;
        }

        public Guid VersionId { get; private set; }

    }
}