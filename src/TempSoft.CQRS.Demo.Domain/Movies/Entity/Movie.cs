using System;
using System.Collections.Generic;
using System.Linq;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Demo.Domain.Movies.Commands;
using TempSoft.CQRS.Demo.Domain.Movies.Events;
using TempSoft.CQRS.Demo.Domain.Movies.Models;
using TempSoft.CQRS.Domain;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.Demo.Domain.Movies.Entity
{
    public class Movie : AggregateRoot<Movie>, IAggregateRootWithReadModel
    {
        private readonly List<Version> _versions = new List<Version>();

        public string PublicId { get; private set; }

        public string Title { get; private set; }

        public IEnumerable<Version> Versions => _versions;

        public IAggregateRootReadModel GetReadModel()
        {
            return new MovieReadModel
            {
                Version = Version,
                Id = Id,
                PublicId = PublicId,
                Title = Title,
                Versions = Versions.Select(v => new VersionReadModel
                {
                    Id = v.Id,
                    Has3D = v.Has3D,
                    Name = v.Name,
                    HasIMAX = v.HasIMAX,
                    HasTHX = v.HasTHX
                }).ToArray()
            };
        }

        [CommandHandler(typeof(InitializeMovie))]
        public void Initialize(Guid aggregateRootId, string publicId, string title)
        {
            ApplyChange(new MovieInitialized(aggregateRootId, publicId, title));
        }

        [CommandHandler(typeof(AddMovieVersion))]
        public void AddMovieVersion(Guid versionId, string name)
        {
            ApplyChange(new AddedMovieVersion(versionId, name));
        }

        [EventHandler(typeof(MovieInitialized))]
        private void Apply(MovieInitialized @event)
        {
            PublicId = @event.PublicId;
            Id = @event.AggregateRootId;
            Title = @event.Title;
        }

        [EventHandler(typeof(AddedMovieVersion))]
        private void Apply(AddedMovieVersion @event)
        {
            var entity = new Version(this, @event.VersionId, @event.Name);
            _versions.Add(entity);
        }
    }
}