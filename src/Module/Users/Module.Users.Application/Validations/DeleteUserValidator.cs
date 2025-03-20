using FluentValidation;
using Module.Users.Application.Commands.DeleteUserCommand;

namespace Module.Users.Application.Validations;

public class DeleteUserValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserValidator()
    {
        RuleFor(u => u.Id)
            .NotNull().WithMessage("Id cannot be empty")
            .NotEmpty().WithMessage("Id cannot be empty");
    }
}