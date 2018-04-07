using System;
using TempSoft.CQRS.Commands;

namespace TempSoft.CQRS.Demo.Domain.Movies.Commands
{
    public class AddMovieVersion : CommandBase
    {
        private AddMovieVersion()
        {
        }

        public AddMovieVersion(Guid versionId, string name)
        {
            VersionId = versionId;
            Name = name;
        }

        public Guid VersionId { get; private set; }

        public string Name { get; private set; }
    }
}