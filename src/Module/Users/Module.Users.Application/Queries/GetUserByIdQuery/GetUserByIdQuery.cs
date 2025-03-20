using MediatR;
using Module.Users.Application.Dtos;
using Shared.Domain.Common;

namespace Module.Users.Application.Queries.GetUserByIdQuery;

public record GetUserByIdQuery(Guid Id) : IRequest<ServiceResult<UserResponseDto>>;