using Microsoft.AspNetCore.Mvc;
using SGE.Application.Features.Auth.Commands;

namespace SGE.API.Controllers;

[Route("api/[controller]")]
public class AuthController : ApiControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        try
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}