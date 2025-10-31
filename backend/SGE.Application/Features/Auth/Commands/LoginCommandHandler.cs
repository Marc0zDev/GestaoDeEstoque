using AutoMapper;
using MediatR;
using SGE.Application.Common.DTOs;
using SGE.Application.Common.Interfaces;
using SGE.Domain.Interfaces;

namespace SGE.Application.Features.Auth.Commands;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponseDto>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordService _passwordService;
    private readonly IJwtService _jwtService;
    private readonly IMapper _mapper;

    public LoginCommandHandler(
        IUsuarioRepository usuarioRepository,
        IPasswordService passwordService,
        IJwtService jwtService,
        IMapper mapper)
    {
        _usuarioRepository = usuarioRepository;
        _passwordService = passwordService;
        _jwtService = jwtService;
        _mapper = mapper;
    }

    public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioRepository.GetByEmailAsync(request.Email, cancellationToken);
        
        if (usuario == null)
            throw new UnauthorizedAccessException("Email ou senha inválidos");

        if (!usuario.Ativo)
            throw new UnauthorizedAccessException("Usuário inativo");

        if (!_passwordService.VerifyPassword(request.Password, usuario.SenhaHash))
            throw new UnauthorizedAccessException("Email ou senha inválidos");

        var token = _jwtService.GenerateToken(usuario.Id, usuario.Email.Value, usuario.Role.ToString());

        return new LoginResponseDto
        {
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddHours(8),
            Usuario = _mapper.Map<UsuarioDto>(usuario)
        };
    }
}