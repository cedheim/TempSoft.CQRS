using System;
using Newtonsoft.Json;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Demo.ValueObjects;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.Demo.Domain.Movies.Events
{
    public class LocalInformationCreated : EventBase
    {
        public LocalInformationCreated(string country, string language)
        {
            Culture = new Culture(country, language);
        }

        [JsonConstructor]
        public LocalInformationCreated(Culture culture)
        {
            Culture = culture;
        }
        
        public Culture Culture { get; private set; }
    }
}