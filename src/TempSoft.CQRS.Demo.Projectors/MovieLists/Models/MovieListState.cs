using TempSoft.CQRS.Projectors;

namespace TempSoft.CQRS.Demo.Projectors.MovieLists
{
    public class MovieListState : IProjection
    {
        public string Id { get; set; }

        public string ProjectorId { get; set; }
    }
}