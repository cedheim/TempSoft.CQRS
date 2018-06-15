using System;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Demo.Domain.Movies.Commands;
using TempSoft.CQRS.Demo.Domain.Movies.Events;
using TempSoft.CQRS.Demo.ValueObjects;
using TempSoft.CQRS.Domain;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.Demo.Domain.Movies.Entities
{
    public class LocalInformation : AggregateRoot<Movie>.Entity<LocalInformation>
    {
        public Culture Culture { get; private set; }

        public string Title { get; private set; }

        public LocalInformation(Movie root, Culture culture) : base(root, culture.ToString())
        {
            Culture = culture;
        }

        [CommandHandler(typeof(SetLocalTitle))]
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