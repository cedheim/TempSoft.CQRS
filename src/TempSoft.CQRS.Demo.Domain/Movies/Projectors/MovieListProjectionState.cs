using TempSoft.CQRS.Projectors;

namespace TempSoft.CQRS.Demo.Domain.Movies.Projectors
{
    public class MovieListProjectionState : IProjection
    {
        public string Id { get; set; }

        public string ProjectorId { get; set; }
    };
}