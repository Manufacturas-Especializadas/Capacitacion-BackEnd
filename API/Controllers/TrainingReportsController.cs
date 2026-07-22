using Application.Features.TrainingReports.Commands;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingReportsController(IMediator mediator) : ControllerBase
    {
        [HttpPost("create")]
        public async Task<IActionResult> CreateTrainingReport([FromForm] CreateTrainingReportCommand command)
        {
            var reportId = await mediator.Send(command);

            return Ok(new { message = "Reporte de entrenamiento creado con éxito", id = reportId });
        }
    }
}