using FluentValidation;
using Module.Products.Application.Dtos;

namespace Module.Products.Application.Validations;

public class RatingDtoValidator : AbstractValidator<RatingDto>
{
    public RatingDtoValidator()
    {
        RuleFor(x => x.Rate)
            .InclusiveBetween(0, 5).WithMessage("The rating must be between 0 and 5.");

        RuleFor(x => x.Count)
            .GreaterThanOrEqualTo(0).WithMessage("The number of ratings cannot be negative.");
    }
}