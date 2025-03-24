using AutoMapper;
using FluentValidation;
using MediatR;
using Module.Products.Application.Dtos;
using Module.Products.Application.Interfaces.Persistence;
using Module.Products.Domain.Entities;
using Module.Products.Domain.ValueObjects;
using Shared.Application.Interfaces.Persistence;
using Shared.Domain.Common;

namespace Module.Products.Application.Commands.CreateProductCommand;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ServiceResult<ProductResponseDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductUnitOfWork _unitOfWork;
    private readonly IValidator<CreateProductCommand> _validator;
    private readonly IMapper _mapper;

    public CreateProductCommandHandler(IProductRepository productRepository, ICategoryRepository categoryRepository,  IProductUnitOfWork unitOfWork, IValidator<CreateProductCommand> validator, IMapper mapper)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<ServiceResult<ProductResponseDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        var category = await _categoryRepository.GetByNameAsync(request.Category, cancellationToken);

        if (category is null)
            return ServiceResult.Failed<ProductResponseDto>(ServiceError.GenericError("NotFound", "Resource not found", "Category not found."));
        
        var product = new Product(
            request.Title,
            request.Price,
            request.Description,
            category.Id,
            request.Image,
            new Rating(request.Rating.Rate, request.Rating.Count));

        await _productRepository.AddAsync(product, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return ServiceResult.Success(_mapper.Map<ProductResponseDto>(product));

    }
}