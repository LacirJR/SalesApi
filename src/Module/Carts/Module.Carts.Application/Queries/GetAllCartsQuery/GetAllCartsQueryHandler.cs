using AutoMapper;
using MediatR;
using Module.Carts.Application.Dtos;
using Module.Carts.Application.Interfaces.Persistence;
using Shared.Domain.Common;
using Shared.Infrastructure.Common;

namespace Module.Carts.Application.Queries.GetAllCartsQuery;

public class GetAllCartsQueryHandler : IRequestHandler<GetAllCartsQuery, ServiceResult<PaginatedList<CartResponseDto>>>
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;

    public GetAllCartsQueryHandler(ICartRepository cartRepository, IMapper mapper)
    {
        _cartRepository = cartRepository;
        _mapper = mapper;
    }

    public async Task<ServiceResult<PaginatedList<CartResponseDto>>> Handle(GetAllCartsQuery request,
        CancellationToken cancellationToken)
    {
        var cartsPaginated = await _cartRepository.GetAllAsync(request.Filter, request.OrderBy, request.Page,
            request.Size, cancellationToken);
        var cartsDto = _mapper.Map<List<CartResponseDto>>(cartsPaginated.Data);

        return ServiceResult.Success(new PaginatedList<CartResponseDto>(cartsDto, cartsPaginated.TotalItems,
            request.Page, request.Size));
    }
}