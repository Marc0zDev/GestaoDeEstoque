using SGE.Domain.Common;
using SGE.Domain.ValueObjects;

namespace SGE.Domain.Entities;

public class Fornecedor : BaseEntity
{
    public string Nome { get; private set; } = string.Empty;
    public Cnpj Cnpj { get; private set; } = null!;
    public string? Telefone { get; private set; }
    public string? Email { get; private set; }
    public string? Endereco { get; private set; }
    public bool Ativo { get; private set; }

    // Navigation properties
    public virtual ICollection<Produto> Produtos { get; private set; } = new List<Produto>();

    // Constructor for EF Core
    private Fornecedor() { }

    private Fornecedor(string nome, Cnpj cnpj, string? telefone = null, 
                      string? email = null, string? endereco = null)
    {
        Nome = nome;
        Cnpj = cnpj;
        Telefone = telefone;
        Email = email;
        Endereco = endereco;
        Ativo = true;
    }

    public static Fornecedor Create(string nome, string cnpj, string? telefone = null, 
                                   string? email = null, string? endereco = null)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome do fornecedor é obrigatório", nameof(nome));

        var cnpjVO = Cnpj.Create(cnpj);

        return new Fornecedor(
            nome.Trim(), 
            cnpjVO, 
            telefone?.Trim(), 
            email?.Trim(), 
            endereco?.Trim()
        );
    }

    public void AtualizarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome do fornecedor é obrigatório", nameof(nome));

        Nome = nome.Trim();
        MarcarComoAtualizado();
    }

    public void AtualizarCnpj(string cnpj)
    {
        Cnpj = Cnpj.Create(cnpj);
        MarcarComoAtualizado();
    }

    public void AtualizarTelefone(string? telefone)
    {
        Telefone = telefone?.Trim();
        MarcarComoAtualizado();
    }

    public void AtualizarEmail(string? email)
    {
        Email = email?.Trim();
        MarcarComoAtualizado();
    }

    public void AtualizarEndereco(string? endereco)
    {
        Endereco = endereco?.Trim();
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