using AutoMapper;
using SGE.Application.Common.DTOs;
using SGE.Domain.Entities;

namespace SGE.Application.Common;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Usuario mappings
        CreateMap<Usuario, UsuarioDto>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Value))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));

        // Categoria mappings
        CreateMap<Categoria, CategoriaDto>()
            .ForMember(dest => dest.CategoriaPai, opt => opt.MapFrom(src => src.CategoriaParent != null ? src.CategoriaParent.Nome : null))
            .ForMember(dest => dest.SubCategorias, opt => opt.MapFrom(src => src.SubCategorias));

        // Fornecedor mappings
        CreateMap<Fornecedor, FornecedorDto>()
            .ForMember(dest => dest.Cnpj, opt => opt.MapFrom(src => src.Cnpj.Value));

        // LocalArmazenagem mappings
        CreateMap<LocalArmazenagem, LocalArmazenagemDto>();

        // Produto mappings
        CreateMap<Produto, ProdutoDto>()
            .ForMember(dest => dest.CategoriaNome, opt => opt.MapFrom(src => src.Categoria != null ? src.Categoria.Nome : string.Empty))
            .ForMember(dest => dest.FornecedorNome, opt => opt.MapFrom(src => src.Fornecedor != null ? src.Fornecedor.Nome : string.Empty))
            .ForMember(dest => dest.QuantidadeTotal, opt => opt.Ignore()); // Será preenchido separadamente

        // EstoqueItem mappings
        CreateMap<EstoqueItem, EstoqueItemDto>()
            .ForMember(dest => dest.ProdutoNome, opt => opt.MapFrom(src => src.Produto != null ? src.Produto.Nome : string.Empty))
            .ForMember(dest => dest.ProdutoCodigo, opt => opt.MapFrom(src => src.Produto != null ? src.Produto.Codigo : string.Empty))
            .ForMember(dest => dest.LocalNome, opt => opt.MapFrom(src => src.LocalArmazenagem != null ? src.LocalArmazenagem.Nome : string.Empty));

        // EstoqueMovimento mappings
        CreateMap<EstoqueMovimento, EstoqueMovimentoDto>()
            .ForMember(dest => dest.TipoMovimento, opt => opt.MapFrom(src => src.TipoMovimento.ToString()))
            .ForMember(dest => dest.ProdutoNome, opt => opt.MapFrom(src => src.EstoqueItem != null && src.EstoqueItem.Produto != null ? src.EstoqueItem.Produto.Nome : string.Empty))
            .ForMember(dest => dest.LocalNome, opt => opt.MapFrom(src => src.EstoqueItem != null && src.EstoqueItem.LocalArmazenagem != null ? src.EstoqueItem.LocalArmazenagem.Nome : string.Empty));
    }
}