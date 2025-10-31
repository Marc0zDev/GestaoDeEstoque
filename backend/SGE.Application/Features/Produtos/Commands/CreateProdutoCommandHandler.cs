using AutoMapper;
using MediatR;
using SGE.Application.Common.DTOs;
using SGE.Domain.Entities;
using SGE.Domain.Interfaces;

namespace SGE.Application.Features.Produtos.Commands;

public class CreateProdutoCommandHandler : IRequestHandler<CreateProdutoCommand, ProdutoDto>
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IFornecedorRepository _fornecedorRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateProdutoCommandHandler(
        IProdutoRepository produtoRepository,
        ICategoriaRepository categoriaRepository,
        IFornecedorRepository fornecedorRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _produtoRepository = produtoRepository;
        _categoriaRepository = categoriaRepository;
        _fornecedorRepository = fornecedorRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ProdutoDto> Handle(CreateProdutoCommand request, CancellationToken cancellationToken)
    {
        // Verificar se código já existe
        if (await _produtoRepository.ExistsWithCodigoAsync(request.Codigo, cancellationToken: cancellationToken))
            throw new InvalidOperationException("Já existe um produto com este código");

        // Verificar se categoria existe
        var categoria = await _categoriaRepository.GetByIdAsync(request.CategoriaId, cancellationToken);
        if (categoria == null)
            throw new InvalidOperationException("Categoria não encontrada");

        // Verificar se fornecedor existe
        var fornecedor = await _fornecedorRepository.GetByIdAsync(request.FornecedorId, cancellationToken);
        if (fornecedor == null)
            throw new InvalidOperationException("Fornecedor não encontrado");

        if (!fornecedor.Ativo)
            throw new InvalidOperationException("Fornecedor inativo");

        var produto = Produto.Create(
            request.Codigo,
            request.Nome,
            request.Descricao,
            request.Preco,
            request.CategoriaId,
            request.FornecedorId,
            request.EstoqueMinimo);

        await _produtoRepository.AddAsync(produto, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        // Recarregar com dados relacionados
        var produtoSalvo = await _produtoRepository.GetByIdAsync(produto.Id, cancellationToken);
        return _mapper.Map<ProdutoDto>(produtoSalvo);
    }
}