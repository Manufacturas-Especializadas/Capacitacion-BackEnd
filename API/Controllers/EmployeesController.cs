using Application.DTOs;
using Application.Features.Employee.Command;
using Application.Features.Employees.Command;
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

        [HttpPut("updateEmployee/{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] UpdateEmployeeDto request)
        {
            var command = new UpdateEmployeeCommand(id, request);
            var success = await mediator.Send(command);

            if (!success) return NotFound(new { message = "Empleado no encontrado para actualizar" });

            return NoContent();
        }

        [HttpDelete("deleteEmployee/{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var command = new DeleteEmployeeCommand(id);
            var success = await mediator.Send(command);

            if (!success) return NotFound(new { message = "Empleado no encontrado para eliminar" });

            return NoContent();
        }
    }
}