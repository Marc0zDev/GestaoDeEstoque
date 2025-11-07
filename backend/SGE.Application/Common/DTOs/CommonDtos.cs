namespace SGE.Application.Common.DTOs;

public class BaseDto
{
    public Guid Id { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }
}

public class UsuarioDto : BaseDto
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool Ativo { get; set; }
}

public class CategoriaDto : BaseDto
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public Guid? CategoriaParentId { get; set; }
    public string? CategoriaPai { get; set; }
    public List<CategoriaDto> SubCategorias { get; set; } = new();
}

public class FornecedorDto : BaseDto
{
    public string Nome { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public string? Endereco { get; set; }
    public bool Ativo { get; set; }
}

public class LocalArmazenagemDto : BaseDto
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public bool Ativo { get; set; }
}

public class ProdutoDto : BaseDto
{
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public decimal Preco { get; set; }
    public Guid CategoriaId { get; set; }
    public string CategoriaNome { get; set; } = string.Empty;
    public Guid FornecedorId { get; set; }
    public string FornecedorNome { get; set; } = string.Empty;
    public int EstoqueMinimo { get; set; }
    public bool Ativo { get; set; }
    public decimal QuantidadeTotal { get; set; }
}

public class EstoqueItemDto : BaseDto
{
    public Guid ProdutoId { get; set; }
    public string ProdutoNome { get; set; } = string.Empty;
    public string ProdutoCodigo { get; set; } = string.Empty;
    public Guid LocalArmazenagemId { get; set; }
    public string LocalNome { get; set; } = string.Empty;
    public decimal Quantidade { get; set; }
    public decimal? PrecoUltimaCompra { get; set; }
    public DateTime? DataUltimaMovimentacao { get; set; }
}

public class EstoqueMovimentoDto : BaseDto
{
    public Guid EstoqueItemId { get; set; }
    public string ProdutoNome { get; set; } = string.Empty;
    public string LocalNome { get; set; } = string.Empty;
    public string TipoMovimento { get; set; } = string.Empty;
    public decimal Quantidade { get; set; }
    public decimal QuantidadeAnterior { get; set; }
    public decimal QuantidadeAtual { get; set; }
    public string? Observacoes { get; set; }
    public DateTime DataMovimento { get; set; }
}

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public UsuarioDto Usuario { get; set; } = new();
}

// DTOs de Criação
public class CreateCategoriaDto
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public bool Ativo { get; set; } = true;
}

public class UpdateCategoriaDto
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public bool Ativo { get; set; } = true;
}

public class CreateFornecedorDto
{
    public string Nome { get; set; } = string.Empty;
    public string? Cnpj { get; set; }
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public string? Endereco { get; set; }
    public bool Ativo { get; set; } = true;
}

public class UpdateFornecedorDto
{
    public string Nome { get; set; } = string.Empty;
    public string? Cnpj { get; set; }
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public string? Endereco { get; set; }
    public bool Ativo { get; set; } = true;
}

public class CreateProdutoDto
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public int EstoqueMinimo { get; set; }
    public int EstoqueMaximo { get; set; }
    public Guid CategoriaId { get; set; }
    public Guid? FornecedorId { get; set; }
    public bool Ativo { get; set; } = true;
}

public class UpdateProdutoDto
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public int EstoqueMinimo { get; set; }
    public int EstoqueMaximo { get; set; }
    public Guid CategoriaId { get; set; }
    public Guid? FornecedorId { get; set; }
    public bool Ativo { get; set; } = true;
}

public class CreateEstoqueMovimentoDto
{
    public Guid ProdutoId { get; set; }
    public string Tipo { get; set; } = string.Empty; // "Entrada" ou "Saida"
    public int Quantidade { get; set; }
    public string? Observacoes { get; set; }
}

public class PaginatedResultDto<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}