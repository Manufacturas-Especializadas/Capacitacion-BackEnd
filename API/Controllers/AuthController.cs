using Application.Features.Auth;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs;
using MediatR;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IMediator mediator) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var result = await mediator.Send(new LoginCommand(request));

            if(result == null)
            {
                return Unauthorized(new { message = "Numero de nómina o contraseña incorrectos, o usuario inactivo" });
            }

            return Ok(result);
        }
    }
}