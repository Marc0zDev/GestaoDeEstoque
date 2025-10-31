using AutoMapper;
using MediatR;
using SGE.Application.Common.DTOs;
using SGE.Domain.Interfaces;

namespace SGE.Application.Features.Produtos.Queries;

public class GetAllProdutosQueryHandler : IRequestHandler<GetAllProdutosQuery, List<ProdutoDto>>
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IMapper _mapper;

    public GetAllProdutosQueryHandler(IProdutoRepository produtoRepository, IMapper mapper)
    {
        _produtoRepository = produtoRepository;
        _mapper = mapper;
    }

    public async Task<List<ProdutoDto>> Handle(GetAllProdutosQuery request, CancellationToken cancellationToken)
    {
        List<Domain.Entities.Produto> produtos;

        if (!string.IsNullOrEmpty(request.Termo))
        {
            produtos = await _produtoRepository.SearchAsync(request.Termo, cancellationToken);
        }
        else if (request.CategoriaId.HasValue)
        {
            produtos = await _produtoRepository.GetByCategoriaAsync(request.CategoriaId.Value, cancellationToken);
        }
        else if (request.FornecedorId.HasValue)
        {
            produtos = await _produtoRepository.GetByFornecedorAsync(request.FornecedorId.Value, cancellationToken);
        }
        else if (request.ApenasAtivos)
        {
            produtos = await _produtoRepository.GetActiveAsync(cancellationToken);
        }
        else
        {
            produtos = await _produtoRepository.GetAllAsync(cancellationToken);
        }

        return _mapper.Map<List<ProdutoDto>>(produtos);
    }
}