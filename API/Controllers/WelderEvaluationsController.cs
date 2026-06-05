using Application.Features.TrainingEvents.Commands;
using Application.Features.TrainingEvents.Queries;
using Application.Features.WelderEvaluations.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WelderEvaluationsController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await _mediator.Send(new GetWelderEvaluationByIdQuery(id)));
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _mediator.Send(new GetAllWelderEvaluationsQuery()));
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] CreateWelderEvaluationCommand commnad)
        {
            var evaluationId = await _mediator.Send(commnad);

            return Ok(new
            {
                Id = evaluationId,
                Message = "Evaluacion guardada exitosamente"
            });
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromForm] UpdateWelderEvaluationCommand command)
        {
            var evaluationId = await _mediator.Send(command);
            return Ok(new
            {
                Id = evaluationId,
                Message = "Evaluacion actualizada exitosamente"
            });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new DeleteWelderEvaluationCommand(id));
            return Ok(new
            {
                Id = id,
                Message = "Evaluacion eliminada exitosamente"
            });
        }
    }
}