using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Demo.Domain.Movies.Commands;
using TempSoft.CQRS.Demo.Domain.Movies.Events;
using TempSoft.CQRS.Demo.Domain.Movies.Models;
using TempSoft.CQRS.Demo.ValueObjects;
using TempSoft.CQRS.Domain;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.Demo.Domain.Movies.Entities
{
    public class Movie : AggregateRoot<Movie>, IAggregateRootWithReadModel
    {
        private readonly List<LocalInformation> _localInformation;
        private readonly List<Identifier> _identifiers;

        public Movie()
        {
            _localInformation = new List<LocalInformation>();
            _identifiers = new List<Identifier>();
        }

        public string OriginalTitle { get; private set; }

        public IEnumerable<LocalInformation> LocalInformation => _localInformation;

        public IEnumerable<Identifier> Identifiers => _identifiers;

        [CommandHandler(typeof(CreateMovie))]
        public void Create(string originalTitle)
        {
            ApplyChange(new MovieCreated(originalTitle));
        }

        [EventHandler(typeof(MovieCreated))]
        public void Created(string originalTitle)
        {
            OriginalTitle = originalTitle;
        }

        [EventHandler(typeof(LocalInformationCreated))]
        public void LocalInformationCreated(Culture culture)
        {
            _localInformation.Add(new LocalInformation(this, culture));
        }

        [EventHandler(typeof(IdentifierCreated))]
        public void IdentifierCreated(string identifierId)
        {
            _identifiers.Add(new Identifier(this, identifierId));
        }

        public IAggregateRootReadModel GetReadModel()
        {
            return new MovieModel
            {
                Id = Id,
                OriginalTitle = OriginalTitle,
                Version = Version,
                LocalInformation = _localInformation.ToDictionary(li => li.Culture.ToString(), li => new LocalInformationModel
                {
                    Title = li.Title
                }),
                Identifiers = _identifiers.ToDictionary(id => id.Id, id => id.Value)
            };
        }

        protected override IEntity MissingEntityHandler(IEntityCommand entityCommand)
        {
            if ((entityCommand is ILocalInformationCommand localInformationCommand))
            {
                ApplyChange(new LocalInformationCreated(localInformationCommand.Culture));
                return _localInformation.FirstOrDefault(li => li.Culture == localInformationCommand.Culture);
            }

            if ((entityCommand is IIdentifierCommand identifierCommand))
            {
                ApplyChange(new IdentifierCreated(identifierCommand.EntityId));
                return _identifiers.FirstOrDefault(li => li.Id == identifierCommand.EntityId);
            }

            return base.MissingEntityHandler(entityCommand);

        }
    }
}