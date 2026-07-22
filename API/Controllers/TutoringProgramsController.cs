using Application.DTOs;
using Application.Features.TutoringProgram.Commands;
using Application.Features.TutoringProgram.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TutoringProgramsController(IMediator mediator) : ControllerBase
    {
        [HttpGet("Form")]
        public async Task<IActionResult> GetFormTemplate()
        {
            var query = new GetTutoringFormQuery();
            var result = await mediator.Send(query);

            return Ok(result);
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetAllTutoringProgramsQuery();
            var result = await mediator.Send(query);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetTutoringProgramByIdQuery(id);
            var result = await mediator.Send(query);

            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateTutoringProgramDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var command = new CreateTutoringProgramCommand(request);
            var result = await mediator.Send(command);

            return Ok(result);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTutoringProgramDto request)
        {
            if (id != request.Id)
                return BadRequest("El ID de la ruta no coincide con el ID del payload.");

            var command = new UpdateTutoringProgramCommand(request);
            var success = await mediator.Send(command);

            if (!success)
                return NotFound($"No se encontró el programa de tutoreo con el ID {id}.");

            return NoContent();
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteTutoringProgramCommand(id);
            var success = await mediator.Send(command);

            if (!success)
                return NotFound($"No se encontró el programa de tutoreo con el ID {id}.");

            return NoContent();
        }
    }
}