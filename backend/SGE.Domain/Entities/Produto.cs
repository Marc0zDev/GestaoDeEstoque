using SGE.Domain.Common;

namespace SGE.Domain.Entities;

public class Produto : BaseEntity
{
    public string Codigo { get; private set; } = string.Empty;
    public string Nome { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }
    public decimal Preco { get; private set; }
    public Guid CategoriaId { get; private set; }
    public Guid FornecedorId { get; private set; }
    public int EstoqueMinimo { get; private set; }
    public bool Ativo { get; private set; }

    // Navigation properties
    public virtual Categoria? Categoria { get; private set; }
    public virtual Fornecedor? Fornecedor { get; private set; }
    public virtual ICollection<EstoqueItem> EstoqueItens { get; private set; } = new List<EstoqueItem>();

    // Constructor for EF Core
    private Produto() { }

    private Produto(string codigo, string nome, string? descricao, decimal preco, 
                   Guid categoriaId, Guid fornecedorId, int estoqueMinimo)
    {
        Codigo = codigo;
        Nome = nome;
        Descricao = descricao;
        Preco = preco;
        CategoriaId = categoriaId;
        FornecedorId = fornecedorId;
        EstoqueMinimo = estoqueMinimo;
        Ativo = true;
    }

    public static Produto Create(string codigo, string nome, string? descricao, decimal preco, 
                                Guid categoriaId, Guid fornecedorId, int estoqueMinimo)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            throw new ArgumentException("Código é obrigatório", nameof(codigo));

        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome do produto é obrigatório", nameof(nome));

        if (categoriaId == Guid.Empty)
            throw new ArgumentException("Categoria é obrigatória", nameof(categoriaId));

        if (fornecedorId == Guid.Empty)
            throw new ArgumentException("Fornecedor é obrigatório", nameof(fornecedorId));

        if (preco < 0)
            throw new ArgumentException("Preço não pode ser negativo", nameof(preco));

        if (estoqueMinimo < 0)
            throw new ArgumentException("Estoque mínimo não pode ser negativo", nameof(estoqueMinimo));

        return new Produto(
            codigo.Trim().ToUpperInvariant(), 
            nome.Trim(), 
            descricao?.Trim(),
            preco,
            categoriaId, 
            fornecedorId,
            estoqueMinimo
        );
    }

    public void AtualizarCodigo(string codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            throw new ArgumentException("Código é obrigatório", nameof(codigo));

        Codigo = codigo.Trim().ToUpperInvariant();
        MarcarComoAtualizado();
    }

    public void AtualizarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome do produto é obrigatório", nameof(nome));

        Nome = nome.Trim();
        MarcarComoAtualizado();
    }

    public void AtualizarDescricao(string? descricao)
    {
        Descricao = descricao?.Trim();
        MarcarComoAtualizado();
    }

    public void AtualizarPreco(decimal preco)
    {
        if (preco < 0)
            throw new ArgumentException("Preço não pode ser negativo", nameof(preco));

        Preco = preco;
        MarcarComoAtualizado();
    }

    public void AtualizarCategoria(Guid categoriaId)
    {
        if (categoriaId == Guid.Empty)
            throw new ArgumentException("Categoria é obrigatória", nameof(categoriaId));

        CategoriaId = categoriaId;
        MarcarComoAtualizado();
    }

    public void AtualizarFornecedor(Guid fornecedorId)
    {
        if (fornecedorId == Guid.Empty)
            throw new ArgumentException("Fornecedor é obrigatório", nameof(fornecedorId));

        FornecedorId = fornecedorId;
        MarcarComoAtualizado();
    }

    public void AtualizarEstoqueMinimo(int estoqueMinimo)
    {
        if (estoqueMinimo < 0)
            throw new ArgumentException("Estoque mínimo não pode ser negativo", nameof(estoqueMinimo));

        EstoqueMinimo = estoqueMinimo;
        MarcarComoAtualizado();
    }

    public void Ativar()
    {
        Ativo = true;
        MarcarComoAtualizado();
    }

    public void Desativar()
    {
        Ativo = false;
        MarcarComoAtualizado();
    }
}