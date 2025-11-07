using MediatR;
using SGE.Application.Features.Relatorios.DTOs;
using SGE.Domain.Enums;

namespace SGE.Application.Features.Relatorios.Commands;

public record GerarRelatorioEstoqueCommand : IRequest<List<RelatorioEstoqueDto>>
{
    public Guid? CategoriaId { get; init; }
    public Guid? FornecedorId { get; init; }
    public Guid? LocalArmazenagemId { get; init; }
    public StatusEstoque? Status { get; init; }
    public bool IncluirSemEstoque { get; init; } = true;
    public DateTime? DataInicio { get; init; }
    public DateTime? DataFim { get; init; }
}

public record GerarRelatorioMovimentacaoCommand : IRequest<List<RelatorioMovimentacaoDto>>
{
    public DateTime DataInicio { get; init; }
    public DateTime DataFim { get; init; }
    public Guid? ProdutoId { get; init; }
    public TipoMovimento? TipoMovimento { get; init; }
    public Guid? UsuarioId { get; init; }
    public Guid? LocalArmazenagemId { get; init; }
}

public record GerarRelatorioGeralCommand : IRequest<RelatorioGeralDto>
{
    public DateTime? DataReferencia { get; init; }
}