using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Demo.Domain.Movies.Commands;
using TempSoft.CQRS.Demo.Domain.Movies.Events;
using TempSoft.CQRS.Demo.Domain.Movies.Exceptions;
using TempSoft.CQRS.Demo.Domain.Movies.Models;
using TempSoft.CQRS.Demo.Domain.Persons.Entities;
using TempSoft.CQRS.Demo.Domain.Persons.Models;
using TempSoft.CQRS.Demo.ValueObjects;
using TempSoft.CQRS.Domain;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.Demo.Domain.Movies.Entities
{
    public class Movie : AggregateRoot<Movie>, IAggregateRootWithReadModel
    {
        private readonly ICommandRouter _router;
        private readonly List<LocalInformation> _localInformation;
        private readonly List<Identifier> _identifiers;
        private readonly Dictionary<string, HashSet<Guid>> _persons;

        public Movie(ICommandRouter router)
        {
            _router = router;
            _localInformation = new List<LocalInformation>();
            _identifiers = new List<Identifier>();
            _persons = new Dictionary<string, HashSet<Guid>>();
        }

        public string OriginalTitle { get; private set; }

        public IEnumerable<LocalInformation> LocalInformation => _localInformation;

        public IEnumerable<Identifier> Identifiers => _identifiers;

        public IDictionary<string, Guid[]> Persons => _persons.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToArray());

        [CommandHandler(typeof(CreateMovie))]
        public void Create(string originalTitle)
        {
            ApplyChange(new MovieCreated(originalTitle));
        }

        [CommandHandler(typeof(AddActor))]
        public async Task AddActor(Guid personId, CancellationToken cancellationToken)
        {
            await AddPerson(personId, "ACTOR", cancellationToken);
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

        [EventHandler(typeof(PersonAdded))]
        public void PersonAdded(Guid personId, string role)
        {
            if (!_persons.ContainsKey(role))
            {
                _persons.Add(role, new HashSet<Guid>());
            }

            _persons[role].Add(personId);
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
                Identifiers = _identifiers.ToDictionary(id => id.Id, id => id.Value),
                Persons = _persons.ToDictionary(p => p.Key, p => p.Value.ToArray())
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

        private async Task AddPerson(Guid personId, string role, CancellationToken cancellationToken)
        {
            var person = await _router.GetReadModel<Person, PersonModel>(personId, cancellationToken);
            if (person?.Id != personId)
            {
                throw new UnableToAddPersonException($"Could not find person {personId}");
            }

            ApplyChange(new PersonAdded(personId, role));
        }


    }
}