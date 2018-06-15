using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Demo.Api.Models;
using TempSoft.CQRS.Demo.Domain.Persons.Commands;
using TempSoft.CQRS.Demo.Domain.Persons.Entities;
using TempSoft.CQRS.Demo.Domain.Persons.Models;
using TempSoft.CQRS.Demo.ValueObjects;

namespace TempSoft.CQRS.Demo.Api.Controllers
{
    [Route("api/[controller]")]
    public class PersonsController : Controller
    {
        private readonly ICommandRouter _router;

        public PersonsController(ICommandRouter router)
        {
            _router = router;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
        {
            var model = await _router.GetReadModel<Person, PersonModel>(id, cancellationToken);
            if (object.ReferenceEquals(model, default(PersonModel)))
            {
                return NotFound();
            }

            return Ok(model);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PersonInputModel person, CancellationToken cancellationToken)
        {
            if (object.ReferenceEquals(person.Name, default(Name)))
            {
                // status code 422, unprocessable entity.
                return StatusCode(422);
            }

            var aggregateRootId = Guid.NewGuid();

            await _router.Handle<Person>(aggregateRootId, new CreatePerson(person.Name), cancellationToken);
            var model = await _router.GetReadModel<Person, PersonModel>(aggregateRootId, cancellationToken);

            return Ok(model);
        }
    }
}