namespace SGE.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; }
    public bool Ativo { get; protected set; } = true;
    public DateTime CriadoEm { get; protected set; }
    public DateTime? AtualizadoEm { get; protected set; }

    protected BaseEntity()
    {
        Id = Guid.NewGuid();
        CriadoEm = DateTime.UtcNow;
    }

    protected BaseEntity(Guid id)
    {
        Id = id;
        CriadoEm = DateTime.UtcNow;
    }

    public void MarcarComoAtualizado()
    {
        AtualizadoEm = DateTime.UtcNow;
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