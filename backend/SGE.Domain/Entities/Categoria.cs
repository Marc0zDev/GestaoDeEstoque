using SGE.Domain.Common;

namespace SGE.Domain.Entities;

public class Categoria : BaseEntity
{
    public string Nome { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }
    public Guid? CategoriaParentId { get; private set; }

    // Navigation properties
    public virtual Categoria? CategoriaParent { get; private set; }
    public virtual ICollection<Categoria> SubCategorias { get; private set; } = new List<Categoria>();
    public virtual ICollection<Produto> Produtos { get; private set; } = new List<Produto>();

    // Constructor for EF Core
    private Categoria() { }

    private Categoria(string nome, string? descricao = null, Guid? categoriaParentId = null)
    {
        Nome = nome;
        Descricao = descricao;
        CategoriaParentId = categoriaParentId;
    }

    public static Categoria Create(string nome, string? descricao = null, Guid? categoriaParentId = null)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome da categoria é obrigatório", nameof(nome));

        return new Categoria(nome.Trim(), descricao?.Trim(), categoriaParentId);
    }

    public void AtualizarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome da categoria é obrigatório", nameof(nome));

        Nome = nome.Trim();
        MarcarComoAtualizado();
    }

    public void AtualizarDescricao(string? descricao)
    {
        Descricao = descricao?.Trim();
        MarcarComoAtualizado();
    }

    public void AtualizarCategoriaParent(Guid? categoriaParentId)
    {
        CategoriaParentId = categoriaParentId;
        MarcarComoAtualizado();
    }
}