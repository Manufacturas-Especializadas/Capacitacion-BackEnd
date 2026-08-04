using Application.Features.TrainingReports.Commands;
using Application.Features.TrainingReports.Queries.GetTrainingReportById;
using Application.Features.TrainingReports.Queries.GetAllTrainingReports;
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

        [HttpGet("getById/{id:int}")]
        public async Task<IActionResult> GetTrainingReportById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    message = "El identificador del reporte no es válido."
                });
            }

            var report = await mediator.Send(
                new GetTrainingReportByIdQuery(id)
            );

            if (report is null)
            {
                return NotFound(new
                {
                    message = $"No se encontró el reporte con ID {id}."
                });
            }

            return Ok(report);
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAllTrainingReports()
        {
            var reports = await mediator.Send(
                new GetAllTrainingReportsQuery()
                );

            return Ok(reports);
        }


    }
}