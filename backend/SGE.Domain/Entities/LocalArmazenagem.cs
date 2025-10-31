using SGE.Domain.Common;

namespace SGE.Domain.Entities;

public class LocalArmazenagem : BaseEntity
{
    public string Nome { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }
    public bool Ativo { get; private set; }

    // Navigation properties
    public virtual ICollection<EstoqueItem> EstoqueItens { get; private set; } = new List<EstoqueItem>();

    // Constructor for EF Core
    private LocalArmazenagem() { }

    private LocalArmazenagem(string nome, string? descricao = null)
    {
        Nome = nome;
        Descricao = descricao;
        Ativo = true;
    }

    public static LocalArmazenagem Create(string nome, string? descricao = null)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome do local de armazenagem é obrigatório", nameof(nome));

        return new LocalArmazenagem(nome.Trim(), descricao?.Trim());
    }

    public void AtualizarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome do local de armazenagem é obrigatório", nameof(nome));

        Nome = nome.Trim();
        MarcarComoAtualizado();
    }

    public void AtualizarDescricao(string? descricao)
    {
        Descricao = descricao?.Trim();
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