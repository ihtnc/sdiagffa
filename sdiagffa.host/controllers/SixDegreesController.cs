using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using sdiagffa.application.commands.findCommand;
using sdiagffa.application.commands.findPersonCommand;
using sdiagffa.application.commands.getFilmCommand;
using sdiagffa.application.commands.getPersonCommand;
using sdiagffa.application.commands.getPlanetCommand;
using sdiagffa.host.models;

namespace sdiagffa.host.controllers
{
    [Route("api")]
    [ApiController]
    public class SixDegreesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SixDegreesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("/details/people/{id}")]
        public async Task<ActionResult> GetPerson(int id)
        {
            var response = await _mediator.Send(new GetPersonCommandRequest { Id = id });
            if (response == null) { return NoContent(); }

            return Ok(response);
        }

        [HttpGet("/details/films/{id}")]
        public async Task<ActionResult> GetFilm(int id)
        {
            var response = await _mediator.Send(new GetFilmCommandRequest { Id = id });
            if (response == null) { return NoContent(); }

            return Ok(response);
        }

        [HttpGet("/details/planets/{id}")]
        public async Task<ActionResult> GetPlanet(int id)
        {
            var response = await _mediator.Send(new GetPlanetCommandRequest { Id = id });
            if (response == null) { return NoContent(); }

            return Ok(response);
        }

        [HttpPost("/find-connection")]
        public async Task<ActionResult> FindConnection([FromBody] FindCommandRequest request)
        {
            var response = await _mediator.Send(request);
            if (response == null) { return NoContent(); }

            return Ok(response);
        }

        [HttpGet("/details/people")]
        public async Task<ActionResult> FindPerson([FromQuery] FindPersonCommandRequest request)
        {
            var commandResponse = await _mediator.Send(request);
            if (commandResponse == null) { return NoContent(); }

            var response = commandResponse.Adapt<FindPersonResponse>();
            response.NextPageUrl = commandResponse.NextPageId != null
                ? $"/details/people?search={request.Search}&page={commandResponse.NextPageId}"
                : null;

            response.PreviousPageUrl  = commandResponse.PreviousPageId != null
                ? $"/details/people?search={request.Search}&page={commandResponse.PreviousPageId}"
                : null;

            return Ok(response);
        }
    }
}
