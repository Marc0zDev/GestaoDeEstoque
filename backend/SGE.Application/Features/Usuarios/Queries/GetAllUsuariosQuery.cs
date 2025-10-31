using MediatR;
using SGE.Application.Common.DTOs;

namespace SGE.Application.Features.Usuarios.Queries;

public class GetAllUsuariosQuery : IRequest<List<UsuarioDto>>
{
}