using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGE.Application.Features.Produtos.Commands;
using SGE.Application.Features.Produtos.Queries;

namespace SGE.API.Controllers;

[Authorize]
[Route("api/[controller]")]
public class ProdutosController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetAllProdutosQuery query)
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

    [HttpPost]
    [Authorize(Roles = "Admin,Gerente")]
    public async Task<IActionResult> Create([FromBody] CreateProdutoCommand command)
    {
        try
        {
            var result = await Mediator.Send(command);
            return CreatedAtAction(nameof(GetAll), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}