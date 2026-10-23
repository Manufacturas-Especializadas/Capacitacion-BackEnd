using Application.Features.TrainingReports.Commands;
using Application.Features.TrainingReports.Queries.GetTrainingReportById;
using Application.Features.TrainingReports.Queries.GetAllTrainingReports;
//using Application.Features.TrainingReports.Commands.UpdateTrainingReport;
using Application.Features.TrainingReports.Queries.GetTrainingReportPdf;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingReportsController(IMediator mediator) : ControllerBase
    {
        [HttpPost("create")]
        public async Task<IActionResult> CreateTrainingReport(
    [FromForm] CreateTrainingReportCommand command
)
        {
            var result =
                await mediator.Send(command);

            return result.Status switch
            {
                CreateTrainingReportStatus.Created =>
                    Ok(new
                    {
                        message = result.Message,
                        id = result.ReportId
                    }),

                CreateTrainingReportStatus.InvalidRequest =>
                    BadRequest(new
                    {
                        message = result.Message
                    }),

                _ =>
                    StatusCode(
                        StatusCodes.Status500InternalServerError
                    )
            };
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

        [HttpGet("pdf/{id:int}")]
        public async Task<IActionResult> DownloadTrainingReportPdf(
    int id,
    CancellationToken cancellationToken = default
)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    message =
                        "El identificador del reporte no es válido."
                });
            }

            var result =
                await mediator.Send(
                    new GetTrainingReportPdfQuery(id),
                    cancellationToken
                );

            if (result is null)
            {
                return NotFound(new
                {
                    message =
                        $"No se encontró el reporte con ID {id}."
                });
            }

            return File(
                result.Content,
                "application/pdf",
                result.FileName
            );
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAllTrainingReports(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 25,
            [FromQuery] string? search = null,
            [FromQuery] string? trainingType = null,
            [FromQuery] int? weekNumber = null,
            CancellationToken cancellationToken = default)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return BadRequest(new { message = "page y pageSize deben ser mayores que cero." });
            }

            if ((long)(page - 1) * pageSize > int.MaxValue)
            {
                return BadRequest(new { message = "La paginación solicitada excede el rango permitido." });
            }

            if (weekNumber is < 1 or > 53)
            {
                return BadRequest(new { message = "weekNumber debe estar entre 1 y 53." });
            }

            var reports = await mediator.Send(
                new GetAllTrainingReportsQuery(page, pageSize, search, trainingType, weekNumber),
                cancellationToken
                );

            return Ok(reports);
        }

        [HttpDelete("delete/{id:int}")]
        public async Task<IActionResult> DeleteTrainingReport(int id, CancellationToken cancellationToken = default)
        {

            if (id <= 0)
            {
                return BadRequest(new
                {
                    message = "El identificador del reporte no es válido."
                });
            }

            var deleted = await mediator.Send( new DeleteTrainingReportCommand(id),cancellationToken );

            if (!deleted)
            {
                return NotFound(new
                {
                    message = $"No se encontró el reporte con ID {id}."
                });
            }


            return Ok(new
            {
                message = "Reporte de entrenamiento eliminado correctamente."
            });
        }

        [HttpPut("update/{id:int}")]
        public async Task<IActionResult> UpdateTrainingReport(
            int id,
            [FromForm] UpdateTrainingReportCommand command,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    message = "El identificador del reporte no es válido."
                });
            }

            command.Id = id;

            var result = await mediator.Send(
                command,
                cancellationToken
            );

            return result.Status switch
            {
                UpdateTrainingReportStatus.Updated =>
                    Ok(new
                    {
                        message = result.Message,
                        id
                    }),

                UpdateTrainingReportStatus.NotFound =>
                    NotFound(new
                    {
                        message = result.Message
                    }),

                UpdateTrainingReportStatus.InvalidRequest =>
                    BadRequest(new
                    {
                        message = result.Message
                    }),

                _ =>
                    StatusCode(
                        StatusCodes.Status500InternalServerError,
                        new
                        {
                            message = "No se pudo actualizar el reporte."
                        }
                    )
            };

        }

    }
}
