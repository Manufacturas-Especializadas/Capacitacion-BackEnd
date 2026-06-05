using Microsoft.AspNetCore.Mvc;
using MediatR;
using Application.Features.WelderEvaluations.Commands;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WelderEvaluationsController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

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
    }
}