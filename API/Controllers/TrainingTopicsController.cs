using Application.Features.TrainingTopics.Commands;
using Application.Features.TrainingTopics.Queries;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs;
using MediatR;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingTopicsController(IMediator mediator) : ControllerBase
    {
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await mediator.Send(new GetAllTrainingTopicsQuery());
            return Ok(result);
        }

        [HttpGet("byType/{trainingType}")]
        public async Task<IActionResult> GetByType(string trainingType)
        {
            var result = await mediator.Send(new GetTrainingTopicsByTypeQuery(trainingType));
            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateTrainingTopicDto request)
        {
            var command = new CreateTrainingTopicCommand(request);
            var result = await mediator.Send(command);
            return Created("", result);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateTrainingTopicDto request)
        {
            var command = new UpdateTrainingTopicCommand(id, request);
            var success = await mediator.Send(command);

            if (!success) return NotFound(new { message = "Tema no encontrado" });
            return NoContent();
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteTrainingTopicCommand(id);
            var success = await mediator.Send(command);

            if (!success) return NotFound(new { message = "Tema no encontrado" });
            return NoContent();
        }
    }
}