using System;
using TempSoft.CQRS.Commands;

namespace TempSoft.CQRS.Demo.Domain.Movies.Commands
{
    public class AddMovieVersion : CommandBase
    {
        private AddMovieVersion() { }

        public AddMovieVersion(Guid versionId)
        {
            VersionId = versionId;
        }

        public Guid VersionId { get; private set; }

    }
}