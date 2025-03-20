using FluentValidation;
using Module.Users.Application.Queries.GetUserByIdQuery;

namespace Module.Users.Application.Validations;

public class GetUserByIdValidator : AbstractValidator<GetUserByIdQuery>
{
    public GetUserByIdValidator()
    {
        RuleFor(u => u.Id)
            .NotNull().WithMessage("Id cannot be empty")
            .NotEmpty().WithMessage("Id cannot be empty");
    }
    
}