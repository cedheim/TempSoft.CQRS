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
        public async Task<IActionResult> GetMovie(Guid id, CancellationToken cancellationToken)
        {
            var movie = await _router.GetReadModel<Movie, MovieModel>(id, cancellationToken);
            if (object.ReferenceEquals(movie, default(MovieModel)))
            {
                NotFound();
            }

            return Ok(movie);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMovie([FromBody] MovieInputModel model, CancellationToken cancellationToken)
        {
            var aggregateRootId = Guid.NewGuid();
            await _router.Handle<Movie>(aggregateRootId, new CreateMovie(model.OriginalTitle), cancellationToken);

            if (!object.ReferenceEquals(model.LocalInformation, default(Dictionary<string, LocalTitleInputModel>)))
            {
                await Task.WhenAll(model.LocalInformation.Select(li => _router.Handle<Movie>(aggregateRootId, new SetLocalTitle(new Culture(li.Key), li.Value.Title), cancellationToken)));
            }

            return await GetMovie(aggregateRootId, cancellationToken);
        }

        [HttpPut("{id}/local/{culture}/title")]
        public async Task<IActionResult> UpdateTitle(Guid id, string culture, [FromBody] LocalTitleInputModel model, CancellationToken cancellationToken)
        {
            await _router.Handle<Movie>(id, new SetLocalTitle(new Culture(culture), model.Title),
                cancellationToken);

            return await GetTitle(id, culture, cancellationToken);
        }

        [HttpGet("{id}/local/{culture}/title")]
        public async Task<IActionResult> GetTitle(Guid id, string culture, CancellationToken cancellationToken)
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

        [HttpPut("{id}/identifier/{type}")]
        public async Task<IActionResult> UpdateIdentifier(Guid id, string type, [FromBody] IdentifierInputModel model, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(type))
            {
                return StatusCode(422, "An identifier type is required.");
            }

            if (string.IsNullOrEmpty(model?.Value))
            {
                return StatusCode(422, "An identifier value is required.");
            }

            type = type.ToUpper().Trim();
            await _router.Handle<Movie>(id, new SetIdentifier(type, model.Value), cancellationToken);
            return await GetIdentifier(id, type, cancellationToken);
        }

        [HttpGet("{id}/identifier/{type}")]
        public async Task<IActionResult> GetIdentifier(Guid id, string type, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(type))
            {
                return StatusCode(422, "An identifier type is required.");
            }

            type = type.ToUpper().Trim();
            var movie = await _router.GetReadModel<Movie, MovieModel>(id, cancellationToken);
            var identifierId = default(string);

            if (movie.Identifiers.ContainsKey(type))
            {
                identifierId = movie.Identifiers[type];
            }

            return Ok(new Dictionary<string, string> {{type, identifierId}});
        }
    }
}