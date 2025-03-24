using AutoMapper;
using MediatR;
using Module.Users.Application.Dtos;
using Module.Users.Application.Interfaces.Persistence;
using Shared.Domain.Common;
using Shared.Infrastructure.Common;

namespace Module.Users.Application.Queries.GetUsersQuery;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, ServiceResult<PaginatedList<UserResponseDto>>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUsersQueryHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<ServiceResult<PaginatedList<UserResponseDto>>> Handle(GetUsersQuery request,
        CancellationToken cancellationToken)
    {
        var usersPaginated = await _userRepository.GetAllAsync(request.Filter, request.OrderBy, request.Page,
            request.Size,
            cancellationToken);

        var usersDto = _mapper.Map<List<UserResponseDto>>(usersPaginated.Data);

        return ServiceResult.Success(new PaginatedList<UserResponseDto>(usersDto, usersPaginated.TotalItems,
            request.Page, request.Size));
    }
}