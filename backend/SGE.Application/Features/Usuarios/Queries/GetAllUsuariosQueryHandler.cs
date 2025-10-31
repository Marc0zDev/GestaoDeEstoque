using AutoMapper;
using MediatR;
using SGE.Application.Common.DTOs;
using SGE.Domain.Interfaces;

namespace SGE.Application.Features.Usuarios.Queries;

public class GetAllUsuariosQueryHandler : IRequestHandler<GetAllUsuariosQuery, List<UsuarioDto>>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IMapper _mapper;

    public GetAllUsuariosQueryHandler(IUsuarioRepository usuarioRepository, IMapper mapper)
    {
        _usuarioRepository = usuarioRepository;
        _mapper = mapper;
    }

    public async Task<List<UsuarioDto>> Handle(GetAllUsuariosQuery request, CancellationToken cancellationToken)
    {
        var usuarios = await _usuarioRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<List<UsuarioDto>>(usuarios);
    }
}