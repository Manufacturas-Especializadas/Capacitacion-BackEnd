using Application.Features.Auth;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs;
using MediatR;
using Application.Features.Users.Queries;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IMediator mediator) : ControllerBase
    {

        [HttpGet("getRoles")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await mediator.Send(new GetRolesQuery());
            return Ok(roles);
        }

        [HttpGet("getUsers")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await mediator.Send(new GetUsersQuery());

            return Ok(users);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateUserDto request)
        {
            var result = await mediator.Send(new CreateUserCommand(request));

            if (!result) return BadRequest(new { message = "No se pudo crear el usuario" });

            return Ok(new { message = "Usuario creado correctamente" });
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update (int id, [FromBody] UpdateUserDto request)
        {
            var result = await mediator.Send(new UpdateUserCommand(id, request));

            if (!result) return NotFound(new { message = "Usuario no encontrado" });

            return Ok(new { message = "Usuario actualizado exitosamente" });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await mediator.Send(new DeleteUserCommand(id));

            if (!result) return NotFound(new { message = "Usuario no encontrado" });

            return Ok(new { message = "Usuario dado de baja exitosamente" });
        }
    }
}