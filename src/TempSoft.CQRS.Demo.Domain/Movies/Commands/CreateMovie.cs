using System;
using TempSoft.CQRS.Commands;

namespace TempSoft.CQRS.Demo.Domain.Movies.Commands
{
    public class CreateMovie : CommandBase
    {
        public CreateMovie(string originalTitle)
        {
            OriginalTitle = originalTitle;
        }

        public string OriginalTitle { get; private set; }
    }
}