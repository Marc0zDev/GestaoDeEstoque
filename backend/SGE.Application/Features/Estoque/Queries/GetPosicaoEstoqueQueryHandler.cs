using AutoMapper;
using MediatR;
using SGE.Application.Common.DTOs;
using SGE.Domain.Interfaces;

namespace SGE.Application.Features.Estoque.Queries;

public class GetPosicaoEstoqueQueryHandler : IRequestHandler<GetPosicaoEstoqueQuery, List<EstoqueItemDto>>
{
    private readonly IEstoqueRepository _estoqueRepository;
    private readonly IMapper _mapper;

    public GetPosicaoEstoqueQueryHandler(IEstoqueRepository estoqueRepository, IMapper mapper)
    {
        _estoqueRepository = estoqueRepository;
        _mapper = mapper;
    }

    public async Task<List<EstoqueItemDto>> Handle(GetPosicaoEstoqueQuery request, CancellationToken cancellationToken)
    {
        List<Domain.Entities.EstoqueItem> itens;

        if (request.ProdutoId.HasValue)
        {
            itens = await _estoqueRepository.GetEstoqueByProdutoAsync(request.ProdutoId.Value, cancellationToken);
        }
        else if (request.LocalArmazenagemId.HasValue)
        {
            itens = await _estoqueRepository.GetEstoqueByLocalAsync(request.LocalArmazenagemId.Value, cancellationToken);
        }
        else
        {
            itens = await _estoqueRepository.GetAllEstoqueItemsAsync(cancellationToken);
        }

        if (request.ApenasComEstoque)
        {
            itens = itens.Where(x => x.Quantidade > 0).ToList();
        }

        return _mapper.Map<List<EstoqueItemDto>>(itens);
    }
}