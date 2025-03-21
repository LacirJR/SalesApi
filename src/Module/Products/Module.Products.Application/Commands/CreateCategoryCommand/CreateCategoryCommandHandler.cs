using AutoMapper;
using FluentValidation;
using MediatR;
using Module.Products.Application.Dtos;
using Module.Products.Application.Interfaces.Persistence;
using Module.Products.Domain.Entities;
using Shared.Application.Interfaces.Persistence;
using Shared.Domain.Common;

namespace Module.Products.Application.Commands.CreateCategoryCommand;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, ServiceResult<CategoryResponseDto>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateCategoryCommand> _validator;
    private readonly IMapper _mapper;

    public CreateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork,
        IValidator<CreateCategoryCommand> validator, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<ServiceResult<CategoryResponseDto>> Handle(CreateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var category = new Category(request.CategoryName);

        await _categoryRepository.AddAsync(category, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return ServiceResult.Success(_mapper.Map<CategoryResponseDto>(category));
    }
}