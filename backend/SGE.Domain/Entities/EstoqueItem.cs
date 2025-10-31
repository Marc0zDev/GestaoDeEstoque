using SGE.Domain.Common;

namespace SGE.Domain.Entities;

public class EstoqueItem : BaseEntity
{
    public Guid ProdutoId { get; private set; }
    public Guid LocalArmazenagemId { get; private set; }
    public decimal Quantidade { get; private set; }
    public decimal? PrecoUltimaCompra { get; private set; }
    public DateTime? DataUltimaMovimentacao { get; private set; }

    // Navigation properties
    public virtual Produto? Produto { get; private set; }
    public virtual LocalArmazenagem? LocalArmazenagem { get; private set; }
    public virtual ICollection<EstoqueMovimento> Movimentos { get; private set; } = new List<EstoqueMovimento>();

    // Constructor for EF Core
    private EstoqueItem() { }

    private EstoqueItem(Guid produtoId, Guid localArmazenagemId, decimal quantidade = 0)
    {
        ProdutoId = produtoId;
        LocalArmazenagemId = localArmazenagemId;
        Quantidade = quantidade;
    }

    public static EstoqueItem Create(Guid produtoId, Guid localArmazenagemId, decimal quantidade = 0)
    {
        if (produtoId == Guid.Empty)
            throw new ArgumentException("ProdutoId é obrigatório", nameof(produtoId));

        if (localArmazenagemId == Guid.Empty)
            throw new ArgumentException("LocalArmazenagemId é obrigatório", nameof(localArmazenagemId));

        if (quantidade < 0)
            throw new ArgumentException("Quantidade não pode ser negativa", nameof(quantidade));

        return new EstoqueItem(produtoId, localArmazenagemId, quantidade);
    }

    public void AdicionarQuantidade(decimal quantidade)
    {
        if (quantidade <= 0)
            throw new ArgumentException("Quantidade deve ser positiva", nameof(quantidade));

        Quantidade += quantidade;
        DataUltimaMovimentacao = DateTime.UtcNow;
        MarcarComoAtualizado();
    }

    public bool TentarSubtrairQuantidade(decimal quantidade)
    {
        if (quantidade <= 0)
            throw new ArgumentException("Quantidade deve ser positiva", nameof(quantidade));

        if (Quantidade < quantidade)
            return false; // Estoque insuficiente

        Quantidade -= quantidade;
        DataUltimaMovimentacao = DateTime.UtcNow;
        MarcarComoAtualizado();
        return true;
    }

    public void AjustarQuantidade(decimal novaQuantidade)
    {
        if (novaQuantidade < 0)
            throw new ArgumentException("Quantidade não pode ser negativa", nameof(novaQuantidade));

        Quantidade = novaQuantidade;
        DataUltimaMovimentacao = DateTime.UtcNow;
        MarcarComoAtualizado();
    }

    public void AtualizarPrecoUltimaCompra(decimal preco)
    {
        if (preco < 0)
            throw new ArgumentException("Preço não pode ser negativo", nameof(preco));

        PrecoUltimaCompra = preco;
        MarcarComoAtualizado();
    }

    public bool EstoqueAbaixoDoMinimo(int estoqueMinimo)
    {
        return Quantidade < estoqueMinimo;
    }
}