using SGE.Domain.Common;
using SGE.Domain.Enums;
using SGE.Domain.ValueObjects;

namespace SGE.Domain.Entities;

public class Usuario : BaseEntity
{
    public string Nome { get; private set; }
    public Email Email { get; private set; }
    public string SenhaHash { get; private set; } = string.Empty;
    public Role Role { get; private set; }
    public bool Ativo { get; private set; }

    // Constructor for EF Core
    private Usuario() { }

    private Usuario(string nome, Email email, string senhaHash, Role role)
    {
        Nome = nome;
        Email = email;
        SenhaHash = senhaHash;
        Role = role;
        Ativo = true;
    }

    public static Usuario Create(string nome, string email, string senhaHash, Role role)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome é obrigatório", nameof(nome));

        if (string.IsNullOrWhiteSpace(senhaHash))
            throw new ArgumentException("Senha hash é obrigatório", nameof(senhaHash));

        var emailVO = Email.Create(email);
        
        return new Usuario(nome.Trim(), emailVO, senhaHash, role);
    }

    public void AtualizarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome é obrigatório", nameof(nome));

        Nome = nome.Trim();
        MarcarComoAtualizado();
    }

    public void AtualizarEmail(string email)
    {
        Email = Email.Create(email);
        MarcarComoAtualizado();
    }

    public void AtualizarSenha(string senhaHash)
    {
        if (string.IsNullOrWhiteSpace(senhaHash))
            throw new ArgumentException("Senha hash é obrigatório", nameof(senhaHash));

        SenhaHash = senhaHash;
        MarcarComoAtualizado();
    }

    public void AtualizarRole(Role role)
    {
        Role = role;
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