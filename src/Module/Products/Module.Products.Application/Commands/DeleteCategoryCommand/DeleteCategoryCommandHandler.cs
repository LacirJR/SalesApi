using AutoMapper;
using FluentValidation;
using MediatR;
using Module.Products.Application.Dtos;
using Module.Products.Application.Interfaces.Persistence;
using Shared.Application.Interfaces.Persistence;
using Shared.Domain.Common;

namespace Module.Products.Application.Commands.DeleteCategoryCommand;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, ServiceResult<CategoryResponseDto>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductRepository _productRepository;
    private readonly IProductUnitOfWork _unitOfWork;
    private readonly IValidator<DeleteCategoryCommand> _validator;
    private readonly IMapper _mapper;

    public DeleteCategoryCommandHandler(ICategoryRepository categoryRepository, IProductRepository productRepository,
        IProductUnitOfWork unitOfWork, IValidator<DeleteCategoryCommand> validator, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<ServiceResult<CategoryResponseDto>> Handle(DeleteCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var category = await _categoryRepository.GetByNameAsync(request.CategoryName, cancellationToken);

        if (category is null)
            return ServiceResult.Failed<CategoryResponseDto>(ServiceError.GenericError("NotFound", "Resource not found",
                "Category not found."));

        bool hasProducts = await _productRepository.AnyByCategoryIdAsync(category.Id, cancellationToken);

        if (hasProducts)
            return ServiceResult.Failed<CategoryResponseDto>(ServiceError.GenericError("Validation",
                "Deletion Not Allowed", "Cannot delete the category because there are products associated with it."));

        var categoryDeleted = await _categoryRepository.RemoveByNameAsync(request.CategoryName, cancellationToken);

        if (categoryDeleted is null)
            return ServiceResult.Failed<CategoryResponseDto>(ServiceError.NotFound);

        await _unitOfWork.CommitAsync(cancellationToken);

        return ServiceResult.Success(_mapper.Map<CategoryResponseDto>(categoryDeleted));
    }
}