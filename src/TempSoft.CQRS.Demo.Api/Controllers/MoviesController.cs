using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Demo.Api.Models;
using TempSoft.CQRS.Demo.Domain.Movies.Commands;
using TempSoft.CQRS.Demo.Domain.Movies.Entities;
using TempSoft.CQRS.Demo.Domain.Movies.Models;
using TempSoft.CQRS.Demo.Domain.Movies.Values;

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

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
        {
            var movie = await _router.GetReadModel<Movie, MovieModel>(id, cancellationToken);
            if (object.ReferenceEquals(movie, default(MovieModel)))
            {
                NotFound();
            }

            return Ok(movie);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] MovieInputModel model, CancellationToken cancellationToken)
        {
            var aggregateRootId = Guid.NewGuid();
            await _router.Handle<Movie>(aggregateRootId, new CreateMovie(model.OriginalTitle), cancellationToken);

            return await Get(aggregateRootId, cancellationToken);
        }

        [HttpPut("{id}/local/{country}/title")]
        public async Task<IActionResult> Put(Guid id, string country, [FromBody] LocalTitleInputModel model, CancellationToken cancellationToken)
        {
            await _router.Handle<Movie>(id, new SetLocalTitle(new Country(country), model.Title), cancellationToken);

            return await Get(id, cancellationToken);
        }

        [HttpGet("{id}/local/{country}/title")]
        public async Task<IActionResult> Get(Guid id, string country, CancellationToken cancellationToken)
        {
            var movie = await _router.GetReadModel<Movie, MovieModel>(id, cancellationToken);
            var countryModel = new Country(country);
            var localInformation = movie?.LocalInformation?.FirstOrDefault(li => li.Country == countryModel);

            if (object.ReferenceEquals(localInformation, default(LocalInformationModel)))
            {
                return Ok(new LocalInformationModel
                {
                    Country = new Country(country)
                });
            }

            return Ok(localInformation);
        }
    }
}