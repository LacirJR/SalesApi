using AutoMapper;
using FluentValidation;
using MassTransit;
using MediatR;
using Module.Products.Application.Dtos;
using Module.Products.Application.Interfaces.Persistence;
using Shared.Application.Interfaces.Persistence;
using Shared.Domain.Common;
using Shared.IntegrationEvents.Contracts.Products;

namespace Module.Products.Application.Commands.DeleteProductCommand;

public class DeleteProductCommandHandler :  IRequestHandler<DeleteProductCommand, ServiceResult<ProductResponseDto>>
{
    private readonly IProductRepository  _productRepository;
    private readonly IProductUnitOfWork _unitOfWork;
    private readonly IValidator<DeleteProductCommand> _validator;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;

    public DeleteProductCommandHandler(IProductRepository productRepository, IProductUnitOfWork unitOfWork, IValidator<DeleteProductCommand> validator, IMapper mapper, IPublishEndpoint publishEndpoint)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<ServiceResult<ProductResponseDto>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        var productDeleted = await _productRepository.RemoveByIdAsync(request.Id, cancellationToken);
        
        if (productDeleted is null)
            return ServiceResult.Failed<ProductResponseDto>(ServiceError.NotFound);
        
        await _unitOfWork.CommitAsync(cancellationToken);
        
        await _publishEndpoint.Publish(new ProductDeletedIntegrationEvent(request.Id), cancellationToken);
        
        return ServiceResult.Success(_mapper.Map<ProductResponseDto>(productDeleted));
    }
}