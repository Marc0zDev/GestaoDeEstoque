using FluentValidation;
using MediatR;
using SGE.Application.Common.DTOs;
using SGE.Domain.Enums;

namespace SGE.Application.Features.Estoque.Commands;

public class MovimentarEstoqueCommand : IRequest<EstoqueMovimentoDto>
{
    public Guid ProdutoId { get; set; }
    public Guid LocalArmazenagemId { get; set; }
    public TipoMovimento TipoMovimento { get; set; }
    public decimal Quantidade { get; set; }
    public string? Observacoes { get; set; }
}

public class MovimentarEstoqueCommandValidator : AbstractValidator<MovimentarEstoqueCommand>
{
    public MovimentarEstoqueCommandValidator()
    {
        RuleFor(x => x.ProdutoId)
            .NotEmpty().WithMessage("Produto é obrigatório");

        RuleFor(x => x.LocalArmazenagemId)
            .NotEmpty().WithMessage("Local de armazenagem é obrigatório");

        RuleFor(x => x.TipoMovimento)
            .IsInEnum().WithMessage("Tipo de movimento deve ser uma opção válida");

        RuleFor(x => x.Quantidade)
            .GreaterThan(0).WithMessage("Quantidade deve ser maior que zero");

        RuleFor(x => x.Observacoes)
            .MaximumLength(500).WithMessage("Observações devem ter no máximo 500 caracteres");
    }
}