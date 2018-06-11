using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Demo.Domain.Movies.Commands;
using TempSoft.CQRS.Demo.Domain.Movies.Events;
using TempSoft.CQRS.Demo.Domain.Movies.Models;
using TempSoft.CQRS.Demo.Domain.Movies.Values;
using TempSoft.CQRS.Domain;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.Demo.Domain.Movies.Entities
{
    public class Movie : AggregateRoot<Movie>, IAggregateRootWithReadModel
    {
        private readonly List<LocalInformation> _localInformation;

        public Movie()
        {
            _localInformation = new List<LocalInformation>();
        }

        public string OriginalTitle { get; private set; }

        public IEnumerable<LocalInformation> LocalInformation => _localInformation;

        [CommandHandler(typeof(CreateMovie))]
        public void Create(string originalTitle)
        {
            ApplyChange(new MovieCreated(originalTitle));
        }

        [CommandHandler(typeof(SetLocalTitle))]
        public void SetLocalTitle(Country country, string title)
        {
            if (_localInformation.All(li => li.Country != country))
            {
                ApplyChange(new LocalInformationCreated(Guid.NewGuid(), country));
            }

            var localInformation = _localInformation.First(li => li.Country == country);
            localInformation.SetTitle(title);
        }

        [EventHandler(typeof(MovieCreated))]
        private void Created(string originalTitle)
        {
            OriginalTitle = originalTitle;
        }

        [EventHandler(typeof(LocalInformationCreated))]
        private void LocalInformationCreated(Guid entityId, Country country)
        {
            _localInformation.Add(new LocalInformation(this, entityId, country));
        }

        public IAggregateRootReadModel GetReadModel()
        {
            return new MovieModel
            {
                Id = Id,
                OriginalTitle = OriginalTitle,
                Version = Version,
                LocalInformation = _localInformation.Select(li => new LocalInformationModel()
                {
                    Country = li.Country,
                    Title = li.Title
                }).ToArray()
            };
        }
    }
}