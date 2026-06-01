using Application.DTOs;
using Application.Features.Employees.Commands;
using Application.Features.TrainingEvents.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController(IMediator mediator) : ControllerBase
    {
        [HttpGet("allEmployees")]
        public async Task<IActionResult> GetAllEmployees()
        {
            var result = await mediator.Send(new GetEmployeesQuery());

            return Ok(result);
        }

        [HttpPost("createEmployee")]
        public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeDto request)
        {
            var commnad = new CreateEmployeeCommand(request);
            var newEmployee = await mediator.Send(commnad);

            return Created("", newEmployee);
        }
    }
}