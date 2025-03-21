using FluentValidation;
using Module.Users.Application.Commands.CreateUserCommand;
using Module.Users.Application.Interfaces.Services;
using Module.Users.Application.Services;

namespace Module.Users.Application.Validations;

public class CreateUserValidator : AbstractValidator<CreateUserCommand>
{
    private IUserDomainService _userDomainService;
    public CreateUserValidator(IUserDomainService  userDomainService)
    {
        _userDomainService = userDomainService;

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.")
            .MustAsync(EmailMustBeUnique).WithMessage("Email is already used.");

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters long.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");

        RuleFor(x => x.Name.Firstname)
            .NotEmpty().WithMessage("Firstname is required.");

        RuleFor(x => x.Name.Lastname)
            .NotEmpty().WithMessage("Lastname is required.");

        RuleFor(x => x.Address.City)
            .NotEmpty().WithMessage("City is required.");

        RuleFor(x => x.Address.Street)
            .NotEmpty().WithMessage("Street is required.");

        RuleFor(x => x.Address.Number)
            .GreaterThan(0).WithMessage("Number must be greater than zero.");

        RuleFor(x => x.Address.Zipcode)
            .NotEmpty().WithMessage("Zipcode is required.");
    }
    
    private async Task<bool> EmailMustBeUnique(string email, CancellationToken cancellationToken)
    {
        var emailExists = await _userDomainService.ValidateEmailIsUniqueAsync(email, cancellationToken);
        return emailExists;
    }
}