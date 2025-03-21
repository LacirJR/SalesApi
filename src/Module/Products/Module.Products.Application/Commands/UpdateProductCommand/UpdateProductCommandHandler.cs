using AutoMapper;
using FluentValidation;
using MediatR;
using Module.Products.Application.Dtos;
using Module.Products.Application.Interfaces.Persistence;
using Module.Products.Domain.ValueObjects;
using Shared.Application.Interfaces.Persistence;
using Shared.Domain.Common;

namespace Module.Products.Application.Commands.UpdateProductCommand;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ServiceResult<ProductResponseDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateProductCommand> _validator;
    private readonly IMapper _mapper;

    public UpdateProductCommandHandler(IProductRepository productRepository, ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork, IValidator<UpdateProductCommand> validator, IMapper mapper)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<ServiceResult<ProductResponseDto>> Handle(UpdateProductCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);

        if (product is null)
            return ServiceResult.Failed<ProductResponseDto>(ServiceError.NotFound);

        var category = await _categoryRepository.GetByNameAsync(request.Category, cancellationToken);

        if (category is null)
            return ServiceResult.Failed<ProductResponseDto>(ServiceError.GenericError("NotFound", "Resource not found",
                "Category not found."));


        product.Update(request.Title, request.Price,
            request.Description, category.Id, request.Image, new Rating(request.Rating.Rate, request.Rating.Count));
        
        _productRepository.Update(product);
        await _unitOfWork.CommitAsync(cancellationToken);
        
        return ServiceResult.Success(_mapper.Map<ProductResponseDto>(product));
    }
}