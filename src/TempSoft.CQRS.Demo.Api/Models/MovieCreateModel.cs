using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TempSoft.CQRS.Demo.Api.Models
{
    public class MovieCreateModel
    {
        public string PublicId { get; set; }
        public string Title { get; set; }
    }
}
