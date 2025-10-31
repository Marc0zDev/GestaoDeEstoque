using MediatR;
using SGE.Application.Common.DTOs;

namespace SGE.Application.Features.Produtos.Queries;

public class GetAllProdutosQuery : IRequest<List<ProdutoDto>>
{
    public bool ApenasAtivos { get; set; } = true;
    public string? Termo { get; set; }
    public Guid? CategoriaId { get; set; }
    public Guid? FornecedorId { get; set; }
}