using System;

namespace TempSoft.CQRS.Demo.Domain.Movies.Entity
{
    public class Version : Movie.Entity<Version>
    {
        public Version(Movie root, Guid id) : base(root, id)
        {
        }
    }
}