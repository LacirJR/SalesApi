using FluentValidation;
using Module.Products.Application.Commands.CreateCategoryCommand;
using Module.Products.Application.Interfaces.Persistence;

namespace Module.Products.Application.Validations;

public class CreateCategoryValidator :  AbstractValidator<CreateCategoryCommand>
{
    private readonly ICategoryRepository _categoryRepository;

    public CreateCategoryValidator(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;

        RuleFor(x => x.CategoryName)
            .NotEmpty().WithMessage("The category name is required.")
            .MustAsync(CategoryMustNotExist).WithMessage("The specified category already exist.");
    }

    private async Task<bool> CategoryMustNotExist(string categoryName, CancellationToken cancellationToken)
    {
        var response = await _categoryRepository.GetByNameAsync(categoryName, cancellationToken) is null;
        return response;
    }
}