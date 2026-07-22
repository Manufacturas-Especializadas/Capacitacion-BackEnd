using Application.Features.TrainingEvents.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogsController(IMediator mediator) : ControllerBase
    {
        [HttpGet("rooms")]
        public async Task<IActionResult> GetRooms()
        {
            return Ok(await mediator.Send(new GetRoomsQuery()));
        }

        [HttpGet("lines")]
        public async Task<IActionResult> GetLines()
        {
            return Ok(await mediator.Send(new GetLinesQuery()));
        }

        [HttpGet("tutors")]
        public async Task<IActionResult> GetTutors()
        {
            return Ok(await mediator.Send(new GetTutors()));
        }

        [HttpGet("weeks")]
        public async Task<IActionResult> GetWeeks()
        {
            return Ok(await mediator.Send(new GetWeek()));
        }
    }
}