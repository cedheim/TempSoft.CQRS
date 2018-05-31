using System;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.Demo.Domain.Movies.Events
{
    public class AddedMovieVersion : EventBase
    {
        private AddedMovieVersion() { }

        public AddedMovieVersion(Guid versionId, string name)
        {
            VersionId = versionId;
            Name = name;
        }

        public Guid VersionId { get; private set; }

        public string Name { get; private set; }

    }
}