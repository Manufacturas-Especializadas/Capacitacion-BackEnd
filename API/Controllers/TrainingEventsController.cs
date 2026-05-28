using Application.DTOs;
using Application.Features.TrainingEvents.Commands;
using Application.Features.TrainingEvents.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingEventsController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost("create-event")]
        public async Task<IActionResult?> CreateEvent([FromBody] CreateTrainingEventDto request)
        {
            var command = new CreateTrainingEventCommand(request);
            var eventId = await _mediator.Send(command);

            return CreatedAtAction(nameof(GetEventDetails), new { id = eventId }, new { eventId });
        }

        [HttpPost("assign-attendees")]
        public async Task<IActionResult> AssignAttendees([FromBody] AssignAttendeesDto request)
        {
            var command = new AssignAttendeesCommand(request);
            var success = await _mediator.Send(command);

            if (!success)
                return BadRequest("No se pudo procesar la asignación de participantes.");

            return Ok(new { message = "Participantes y matriz asignados correctamente." });
        }

        [HttpGet("details/{id}")]
        public async Task<IActionResult> GetEventDetails(int id)
        {
            var query = new GetTrainingEventQuery(id);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound(new { message = $"No se encontró el evento con ID {id}." });

            return Ok(result);
        }
    }
}