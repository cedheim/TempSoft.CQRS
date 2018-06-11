using System;
using TempSoft.CQRS.Demo.Domain.Movies.Events;
using TempSoft.CQRS.Demo.Domain.Movies.Values;
using TempSoft.CQRS.Domain;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.Demo.Domain.Movies.Entities
{
    public class LocalInformation : AggregateRoot<Movie>.Entity<LocalInformation>
    {
        public Country Country { get; private set; }

        public string Title { get; private set; }

        public LocalInformation(Movie root, Guid id, Country country) : base(root, id)
        {
            Country = country;
        }

        public void SetTitle(string title)
        {
            ApplyChange(new LocalTitleSet(title));
        }

        [EventHandler(typeof(LocalTitleSet))]
        public void TitleSet(string title)
        {
            Title = title;
        }
    } 
}