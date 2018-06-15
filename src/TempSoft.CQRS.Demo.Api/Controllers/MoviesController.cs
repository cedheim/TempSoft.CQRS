using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Demo.Api.Models;
using TempSoft.CQRS.Demo.Domain.Movies.Commands;
using TempSoft.CQRS.Demo.Domain.Movies.Entities;
using TempSoft.CQRS.Demo.Domain.Movies.Models;
using TempSoft.CQRS.Demo.ValueObjects;

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

            if (!object.ReferenceEquals(model.LocalInformation, default(Dictionary<string, LocalTitleInputModel>)))
            {
                await Task.WhenAll(model.LocalInformation.Select(li => _router.Handle<Movie>(aggregateRootId, new SetLocalTitle(new Culture(li.Key), li.Value.Title), cancellationToken)));
            }

            return await Get(aggregateRootId, cancellationToken);
        }

        [HttpPut("{id}/local/{culture}/title")]
        public async Task<IActionResult> Put(Guid id, string culture, [FromBody] LocalTitleInputModel model, CancellationToken cancellationToken)
        {
            await _router.Handle<Movie>(id, new SetLocalTitle(new Culture(culture), model.Title),
                cancellationToken);

            return await Get(id, cancellationToken);
        }

        [HttpGet("{id}/local/{culture}/title")]
        public async Task<IActionResult> Get(Guid id, string culture, CancellationToken cancellationToken)
        {
            var movie = await _router.GetReadModel<Movie, MovieModel>(id, cancellationToken);
            var cultureModel = new Culture(culture);
            var localInformationDictionary = movie?.LocalInformation;

            if (object.ReferenceEquals(localInformationDictionary, default(Dictionary<string, LocalInformationModel>)) || !localInformationDictionary.ContainsKey(cultureModel.ToString()))
            {
                return Ok(new LocalInformationModel());
            }

            var localInformation = localInformationDictionary[cultureModel.ToString()];

            return Ok(localInformation);
        }
    }
}