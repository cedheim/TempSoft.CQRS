using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Demo.Domain.Movies.Commands;
using TempSoft.CQRS.Demo.Domain.Movies.Events;
using TempSoft.CQRS.Domain;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.Demo.Domain.Movies.Entity
{
    public class Movie : AggregateRoot<Movie>
    {
        private readonly List<Version> _versions = new List<Version>();

        public string PublicId { get; private set; }

        public IEnumerable<Version> Versions => _versions;

        [CommandHandler(typeof(InitializeMovie))]
        public void Initialize(Guid aggregateRootId, string publicId)
        {
            ApplyChange(new MovieInitialized(aggregateRootId, publicId));
        }

        [CommandHandler(typeof(AddMovieVersion))]
        public void AddMovieVersion(Guid versionId)
        {
            ApplyChange(new AddedMovieVersion(versionId));
        }

        [EventHandler(typeof(MovieInitialized))]
        private void Apply(MovieInitialized @event)
        {
            PublicId = @event.PublicId;
            Id = @event.AggregateRootId;

            
        }

        [EventHandler(typeof(AddedMovieVersion))]
        private void Apply(AddedMovieVersion @event)
        {
            var entity = new Version(this, @event.VersionId);
            _versions.Add(entity);
        }
    }
}