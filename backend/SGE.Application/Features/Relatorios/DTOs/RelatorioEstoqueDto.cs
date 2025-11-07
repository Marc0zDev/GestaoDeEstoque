using SGE.Domain.Enums;

namespace SGE.Application.Features.Relatorios.DTOs;

public record RelatorioEstoqueDto
{
    public Guid ProdutoId { get; init; }
    public string CodigoProduto { get; init; } = string.Empty;
    public string NomeProduto { get; init; } = string.Empty;
    public string Categoria { get; init; } = string.Empty;
    public int QuantidadeAtual { get; init; }
    public int EstoqueMinimo { get; init; }
    public int EstoqueMaximo { get; init; }
    public decimal ValorUnitario { get; init; }
    public decimal ValorTotalEstoque { get; init; }
    public string LocalArmazenagem { get; init; } = string.Empty;
    public DateTime UltimaMovimentacao { get; init; }
    public StatusEstoque Status { get; init; }
}

public enum StatusEstoque
{
    Normal,
    EstoqueMinimo,
    EstoqueCritico,
    SemEstoque,
    ExcessoEstoque
}

public record RelatorioMovimentacaoDto
{
    public DateTime Data { get; init; }
    public string CodigoProduto { get; init; } = string.Empty;
    public string NomeProduto { get; init; } = string.Empty;
    public TipoMovimento Tipo { get; init; }
    public int Quantidade { get; init; }
    public string Observacoes { get; init; } = string.Empty;
    public string Usuario { get; init; } = string.Empty;
    public string LocalArmazenagem { get; init; } = string.Empty;
}

public record RelatorioGeralDto
{
    public int TotalProdutos { get; init; }
    public int ProdutosComEstoque { get; init; }
    public int ProdutosSemEstoque { get; init; }
    public int ProdutosEstoqueMinimo { get; init; }
    public int ProdutosEstoqueCritico { get; init; }
    public decimal ValorTotalEstoque { get; init; }
    public int TotalMovimentacoesHoje { get; init; }
    public int TotalMovimentacoesMes { get; init; }
    public List<RelatorioEstoqueDto> ProdutosCriticos { get; init; } = [];
    public List<RelatorioMovimentacaoDto> UltimasMovimentacoes { get; init; } = [];
}