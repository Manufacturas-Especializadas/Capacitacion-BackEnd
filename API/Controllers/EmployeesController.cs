using Application.DTOs;
using Application.Features.Employee.Command;
using Application.Features.Employees.Queries;
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

        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var result = await mediator.Send(new GetEmployeeByIdQuery(id));

            if (result == null) return NotFound(new { message = "Empleado no encontrado" });

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