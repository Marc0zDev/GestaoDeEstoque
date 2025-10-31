using MediatR;
using SGE.Application.Common.DTOs;

namespace SGE.Application.Features.Estoque.Queries;

public class GetPosicaoEstoqueQuery : IRequest<List<EstoqueItemDto>>
{
    public Guid? ProdutoId { get; set; }
    public Guid? LocalArmazenagemId { get; set; }
    public bool ApenasComEstoque { get; set; } = false;
}