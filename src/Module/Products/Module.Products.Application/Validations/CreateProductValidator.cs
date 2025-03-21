using FluentValidation;
using Module.Products.Application.Commands.CreateProductCommand;
using Module.Products.Application.Interfaces.Persistence;

namespace Module.Products.Application.Validations;

public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    
    public CreateProductValidator( IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("The product title is required.")
            .MaximumLength(255).WithMessage("The title must not exceed 255 characters.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("The product price must be greater than zero.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("The product description is required.")
            .MaximumLength(1000).WithMessage("The description must not exceed 1000 characters.");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("The product category is required.")
            .MustAsync(CategoryExists).WithMessage("The specified category does not exist.");
        
        RuleFor(x => x)
            .MustAsync(NotExistProductWithSameTitleAndCategory)
            .WithMessage("A product with the same title already exists in this category.");
        
        RuleFor(x => x.Image)
            .NotEmpty().WithMessage("The product image URL is required.")
            .Must(BeAValidUrl).WithMessage("The image URL is invalid.");

        RuleFor(x => x.Rating)
            .SetValidator(new RatingDtoValidator());
    }

    private bool BeAValidUrl(string imageUrl)
    {
        return Uri.TryCreate(imageUrl, UriKind.Absolute, out _);
    }
    
    private async Task<bool> CategoryExists(string categoryName, CancellationToken cancellationToken)
    {
        return await _categoryRepository.ExistsByNameAsync(categoryName, cancellationToken);
    }
    
    private async Task<bool> NotExistProductWithSameTitleAndCategory(CreateProductCommand command, CancellationToken cancellationToken)
    {
        return !await _productRepository.ExistsByTitleAndCategoryAsync(command.Title, command.Category, cancellationToken);
    }
}