using System;
using Newtonsoft.Json;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.Demo.Domain.Movies.Events
{
    public class MovieCreated : EventBase, IInitializationEvent
    {
        [JsonConstructor]
        public MovieCreated(string originalTitle)
            : base()
        {
            OriginalTitle = originalTitle;
        }

        public string OriginalTitle { get; private set; }
    }
}