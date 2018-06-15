using System.Collections.Generic;
using TempSoft.CQRS.Demo.Domain.Movies.Models;

namespace TempSoft.CQRS.Demo.Api.Models
{
    public class MovieInputModel
    {
        public string OriginalTitle { get; set; }

        public Dictionary<string, LocalTitleInputModel> LocalInformation { get; set; }
    }
}