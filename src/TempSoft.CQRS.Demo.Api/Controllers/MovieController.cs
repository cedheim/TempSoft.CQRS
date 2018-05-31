using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Demo.Api.Models;
using TempSoft.CQRS.Demo.Domain.Movies.Commands;
using TempSoft.CQRS.Demo.Domain.Movies.Entity;
using TempSoft.CQRS.Demo.Domain.Movies.Models;

namespace TempSoft.CQRS.Demo.Api.Controllers
{
    [Route("api/[controller]")]
    public class MoviesController : Controller
    {
        private readonly ICommandRouter _router;

        public MoviesController(ICommandRouter router)
        {
            _router = router;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] MovieCreateModel body, CancellationToken cancellationToken)
        {
            var movieId = Guid.NewGuid();
            await _router.Handle<Movie>(movieId, new InitializeMovie(movieId, body.PublicId, body.Title), cancellationToken);
            return Ok(await _router.GetReadModel<Movie, MovieReadModel>(movieId, cancellationToken));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> Get(Guid id, CancellationToken cancellationToken)
        {
            var movie = await _router.GetReadModel<Movie, MovieReadModel>(id, cancellationToken);
            if (movie == null)
                return NotFound();

            return Ok(movie);
        }
    }
}
