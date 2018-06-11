using System;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.Demo.Domain.Movies.Events
{
    public class MovieCreated : EventBase, IInitializationEvent
    {
        private MovieCreated()
        {
        }

        public MovieCreated(string originalTitle)
            : base()
        {
            OriginalTitle = originalTitle;
        }

        public string OriginalTitle { get; private set; }
    }
}