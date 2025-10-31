using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGE.Application.Features.Estoque.Commands;
using SGE.Application.Features.Estoque.Queries;

namespace SGE.API.Controllers;

[Authorize]
[Route("api/[controller]")]
public class EstoqueController : ApiControllerBase
{
    [HttpGet("posicao")]
    public async Task<IActionResult> GetPosicao([FromQuery] GetPosicaoEstoqueQuery query)
    {
        try
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("movimentacao")]
    [Authorize(Roles = "Admin,Gerente,Operador")]
    public async Task<IActionResult> Movimentar([FromBody] MovimentarEstoqueCommand command)
    {
        try
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}