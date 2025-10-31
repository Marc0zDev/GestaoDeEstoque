using AutoMapper;
using MediatR;
using SGE.Application.Common.DTOs;
using SGE.Application.Common.Interfaces;
using SGE.Domain.Entities;
using SGE.Domain.Interfaces;
using SGE.Domain.ValueObjects;

namespace SGE.Application.Features.Usuarios.Commands;

public class CreateUsuarioCommandHandler : IRequestHandler<CreateUsuarioCommand, UsuarioDto>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordService _passwordService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateUsuarioCommandHandler(
        IUsuarioRepository usuarioRepository,
        IPasswordService passwordService,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _usuarioRepository = usuarioRepository;
        _passwordService = passwordService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<UsuarioDto> Handle(CreateUsuarioCommand request, CancellationToken cancellationToken)
    {
        // Verificar se email já existe
        if (await _usuarioRepository.ExistsWithEmailAsync(request.Email, cancellationToken))
            throw new InvalidOperationException("Já existe um usuário com este email");

        var senhaHash = _passwordService.HashPassword(request.Password);

        var usuario = Usuario.Create(
            request.Nome,
            request.Email,
            senhaHash,
            request.Role);

        await _usuarioRepository.AddAsync(usuario, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return _mapper.Map<UsuarioDto>(usuario);
    }
}