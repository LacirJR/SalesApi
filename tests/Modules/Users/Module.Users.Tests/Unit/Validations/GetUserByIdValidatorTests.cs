using FluentValidation.TestHelper;
using Module.Users.Application.Queries.GetUserByIdQuery;
using Module.Users.Application.Validations;
using Xunit;

namespace Module.Users.Tests.Unit.Validations;

public class GetUserByIdValidatorTests 
{
    private readonly GetUserByIdValidator _validator;

    public GetUserByIdValidatorTests()
    {
        _validator = new GetUserByIdValidator();
    }
    
    [Fact]
    public async Task Should_Have_Error_When_Guid_Is_Empty()
    {
        var command = new GetUserByIdQuery(Guid.Empty);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Id).WithErrorMessage("Id cannot be empty");
    }
}