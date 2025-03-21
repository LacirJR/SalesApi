using FluentValidation;
using Module.Products.Application.Commands.DeleteProductCommand;

namespace Module.Products.Application.Validations;

public class DeleteProductValidator : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required");
    }
}