using TempSoft.CQRS.Projectors;

namespace TempSoft.CQRS.Demo.Domain.Movies.Projectors
{
    public class MovieListItemProjection : IProjection
    {
        public string PublicId { get; set; }
        public string Title { get; set; }
        public string Id { get; set; }
        public string ProjectorId { get; set; }
    }
}