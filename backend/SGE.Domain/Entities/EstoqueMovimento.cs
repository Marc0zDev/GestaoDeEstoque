using SGE.Domain.Common;
using SGE.Domain.Enums;

namespace SGE.Domain.Entities;

public class EstoqueMovimento : BaseEntity
{
    public Guid EstoqueItemId { get; private set; }
    public TipoMovimento TipoMovimento { get; private set; }
    public decimal Quantidade { get; private set; }
    public decimal QuantidadeAnterior { get; private set; }
    public decimal QuantidadeAtual { get; private set; }
    public string? Observacoes { get; private set; }
    public DateTime DataMovimento { get; private set; }

    // Navigation properties
    public virtual EstoqueItem? EstoqueItem { get; private set; }

    // Constructor for EF Core
    private EstoqueMovimento() { }

    private EstoqueMovimento(Guid estoqueItemId, TipoMovimento tipoMovimento, decimal quantidade,
                            decimal quantidadeAnterior, decimal quantidadeAtual, string? observacoes = null)
    {
        EstoqueItemId = estoqueItemId;
        TipoMovimento = tipoMovimento;
        Quantidade = quantidade;
        QuantidadeAnterior = quantidadeAnterior;
        QuantidadeAtual = quantidadeAtual;
        Observacoes = observacoes;
        DataMovimento = DateTime.UtcNow;
    }

    public static EstoqueMovimento Create(Guid estoqueItemId, TipoMovimento tipoMovimento, 
                                         decimal quantidade, decimal quantidadeAnterior, 
                                         decimal quantidadeAtual, string? observacoes = null)
    {
        if (estoqueItemId == Guid.Empty)
            throw new ArgumentException("EstoqueItemId é obrigatório", nameof(estoqueItemId));

        if (quantidade <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero", nameof(quantidade));

        if (quantidadeAnterior < 0)
            throw new ArgumentException("Quantidade anterior não pode ser negativa", nameof(quantidadeAnterior));

        if (quantidadeAtual < 0)
            throw new ArgumentException("Quantidade atual não pode ser negativa", nameof(quantidadeAtual));

        return new EstoqueMovimento(
            estoqueItemId, 
            tipoMovimento, 
            quantidade, 
            quantidadeAnterior, 
            quantidadeAtual, 
            observacoes?.Trim()
        );
    }

    public bool IsEntrada() => TipoMovimento == TipoMovimento.Entrada;

    public bool IsSaida() => TipoMovimento == TipoMovimento.Saida;

    public bool IsAjuste() => TipoMovimento == TipoMovimento.Ajuste;
}