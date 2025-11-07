using MediatR;
using SGE.Application.Features.Relatorios.DTOs;

namespace SGE.Application.Features.Relatorios.Queries;

/// <summary>
/// Query para obter dados de relatório de estoque
/// </summary>
public class GetRelatorioEstoqueQuery : IRequest<List<RelatorioEstoqueDto>>
{
    /// <summary>
    /// ID da categoria para filtrar (opcional)
    /// </summary>
    public int? CategoriaId { get; set; }

    /// <summary>
    /// ID do local de armazenagem para filtrar (opcional)
    /// </summary>
    public int? LocalArmazenagemId { get; set; }

    /// <summary>
    /// Incluir apenas produtos com estoque crítico
    /// </summary>
    public bool ApenasEstoqueCritico { get; set; }

    /// <summary>
    /// Data de referência para o relatório (padrão: data atual)
    /// </summary>
    public DateTime DataReferencia { get; set; } = DateTime.Now;
}