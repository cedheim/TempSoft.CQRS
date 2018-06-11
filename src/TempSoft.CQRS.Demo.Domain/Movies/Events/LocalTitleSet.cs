using System;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.Demo.Domain.Movies.Events
{
    public class LocalTitleSet : EntityEventBase
    {
        private LocalTitleSet()
        {
        }

        public LocalTitleSet(string title)
        {
            Title = title;
        }

        public string Title { get; private set; }
    }
}