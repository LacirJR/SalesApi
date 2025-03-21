using FluentValidation;
using Module.Products.Application.Commands.DeleteCategoryCommand;
using Module.Products.Application.Interfaces.Persistence;

namespace Module.Products.Application.Validations;

public class DeleteCategoryValidator : AbstractValidator<DeleteCategoryCommand>
{
    private readonly ICategoryRepository _categoryRepository;

    public DeleteCategoryValidator(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;

        RuleFor(x => x.CategoryName)
            .NotEmpty().WithMessage("The category name is required.")
            .MustAsync(CategoryExists).WithMessage("The specified category does not exist.");
    }

    private async Task<bool> CategoryExists(string categoryName, CancellationToken cancellationToken)
    {
        return await _categoryRepository.GetByNameAsync(categoryName, cancellationToken) is not null;
    }
}