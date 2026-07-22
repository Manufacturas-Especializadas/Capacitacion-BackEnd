using Application.Features.WeldingReportUnionTypes.Queries;
using Application.Features.WeldingReportUnionTypes.Commands;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs;
using MediatR;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeldingReportUnionTypesController(IMediator mediator) : ControllerBase
    {
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await mediator.Send(new GetAllWeldingReportUnionTypesQuery());
            return Ok(result);
        }

        [HttpGet("byReport/{reportId}")]
        public async Task<IActionResult> GetByReportId(int reportId)
        {
            var result = await mediator.Send(new GetWeldingReportUnionTypesByReportIdQuery(reportId));
            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateWeldingReportUnionTypeDto request)
        {
            var command = new CreateWeldingReportUnionTypeCommand(request);
            var result = await mediator.Send(command);
            return Created("", result);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateWeldingReportUnionTypeDto request)
        {
            var command = new UpdateWeldingReportUnionTypeCommand(id, request);
            var success = await mediator.Send(command);

            if (!success) return NotFound(new { message = "Tipo de unión no encontrado" });
            return NoContent();
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteWeldingReportUnionTypeCommand(id);
            var success = await mediator.Send(command);

            if (!success) return NotFound(new { message = "Tipo de unión no encontrado" });
            return NoContent();
        }
    }
}