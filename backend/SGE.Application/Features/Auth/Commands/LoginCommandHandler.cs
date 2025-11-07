using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        IUsuarioRepository usuarioRepository,
        IPasswordService passwordService,
        IJwtService jwtService,
        IMapper mapper,
        ILogger<LoginCommandHandler> logger)
    {
        _usuarioRepository = usuarioRepository;
        _passwordService = passwordService;
        _jwtService = jwtService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Login tentativa para email: {Email}", request.Email);
        
        var usuario = await _usuarioRepository.GetByEmailAsync(request.Email, cancellationToken);
        
        if (usuario == null)
        {
            _logger.LogWarning("Usuário não encontrado para email: {Email}", request.Email);
            throw new UnauthorizedAccessException("Email ou senha inválidos");
        }

        _logger.LogInformation("Usuário encontrado: {Nome}, Ativo: {Ativo}, SenhaHash começa com: {HashPrefix}", 
            usuario.Nome, usuario.Ativo, usuario.SenhaHash.Substring(0, Math.Min(10, usuario.SenhaHash.Length)));

        if (!usuario.Ativo)
        {
            _logger.LogWarning("Usuário inativo: {Email}", request.Email);
            throw new UnauthorizedAccessException("Usuário inativo");
        }

        var passwordValid = _passwordService.VerifyPassword(request.Password, usuario.SenhaHash);
        _logger.LogInformation("Verificação de senha para {Email}: {Valid}", request.Email, passwordValid);
        
        if (!passwordValid)
        {
            _logger.LogWarning("Senha inválida para email: {Email}", request.Email);
            throw new UnauthorizedAccessException("Email ou senha inválidos");
        }

        var token = _jwtService.GenerateToken(usuario.Id, usuario.Email.Value, usuario.Role.ToString());

        return new LoginResponseDto
        {
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddHours(8),
            Usuario = _mapper.Map<UsuarioDto>(usuario)
        };
    }
}