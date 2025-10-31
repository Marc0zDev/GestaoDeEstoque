using AutoMapper;
using MediatR;
using SGE.Application.Common.DTOs;
using SGE.Domain.Entities;
using SGE.Domain.Enums;
using SGE.Domain.Interfaces;

namespace SGE.Application.Features.Estoque.Commands;

public class MovimentarEstoqueCommandHandler : IRequestHandler<MovimentarEstoqueCommand, EstoqueMovimentoDto>
{
    private readonly IEstoqueRepository _estoqueRepository;
    private readonly IProdutoRepository _produtoRepository;
    private readonly ILocalArmazenagemRepository _localRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public MovimentarEstoqueCommandHandler(
        IEstoqueRepository estoqueRepository,
        IProdutoRepository produtoRepository,
        ILocalArmazenagemRepository localRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _estoqueRepository = estoqueRepository;
        _produtoRepository = produtoRepository;
        _localRepository = localRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<EstoqueMovimentoDto> Handle(MovimentarEstoqueCommand request, CancellationToken cancellationToken)
    {
        // Verificar se produto existe e está ativo
        var produto = await _produtoRepository.GetByIdAsync(request.ProdutoId, cancellationToken);
        if (produto == null)
            throw new InvalidOperationException("Produto não encontrado");

        if (!produto.Ativo)
            throw new InvalidOperationException("Produto inativo");

        // Verificar se local existe e está ativo
        var local = await _localRepository.GetByIdAsync(request.LocalArmazenagemId, cancellationToken);
        if (local == null)
            throw new InvalidOperationException("Local de armazenagem não encontrado");

        if (!local.Ativo)
            throw new InvalidOperationException("Local de armazenagem inativo");

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // Buscar ou criar item de estoque
            var estoqueItem = await _estoqueRepository.GetEstoqueItemAsync(
                request.ProdutoId, 
                request.LocalArmazenagemId, 
                cancellationToken);

            if (estoqueItem == null)
            {
                if (request.TipoMovimento == TipoMovimento.Saida)
                    throw new InvalidOperationException("Não é possível fazer saída de produto sem estoque");

                estoqueItem = EstoqueItem.Create(request.ProdutoId, request.LocalArmazenagemId);
                await _estoqueRepository.AddEstoqueItemAsync(estoqueItem, cancellationToken);
            }

            var quantidadeAnterior = estoqueItem.Quantidade;

            // Executar movimentação
            if (request.TipoMovimento == TipoMovimento.Entrada)
            {
                estoqueItem.AdicionarQuantidade(request.Quantidade);
            }
            else if (request.TipoMovimento == TipoMovimento.Saida)
            {
                if (!estoqueItem.TentarSubtrairQuantidade(request.Quantidade))
                    throw new InvalidOperationException("Quantidade insuficiente em estoque");
            }
            else if (request.TipoMovimento == TipoMovimento.Ajuste)
            {
                estoqueItem.AjustarQuantidade(request.Quantidade);
            }

            // Criar movimento
            var movimento = EstoqueMovimento.Create(
                estoqueItem.Id,
                request.TipoMovimento,
                request.Quantidade,
                quantidadeAnterior,
                estoqueItem.Quantidade,
                request.Observacoes);

            await _estoqueRepository.UpdateEstoqueItemAsync(estoqueItem, cancellationToken);
            await _estoqueRepository.AddMovimentoAsync(movimento, cancellationToken);

            await _unitOfWork.CommitAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return _mapper.Map<EstoqueMovimentoDto>(movimento);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}