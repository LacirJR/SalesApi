using MediatR;
using Module.Users.Application.Dtos;
using Shared.Domain.Common;
using Shared.Infrastructure.Common;

namespace Module.Users.Application.Queries.GetUsersQuery;

public class GetUsersQuery: IRequest<ServiceResult<PaginatedList<UserResponseDto>>>
{
    public string? Filter { get; set; }
    public string? OrderBy { get; set; }
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;
    
}