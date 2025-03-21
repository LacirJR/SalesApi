using FluentValidation.TestHelper;
using Module.Products.Application.Dtos;
using Module.Products.Application.Validations;
using Xunit;

namespace Module.Products.Tests.Unit.Validations;

public class RatingDtoValidatorTests
{
    private readonly RatingDtoValidator _validator;

    public RatingDtoValidatorTests()
    {
        _validator = new RatingDtoValidator();
    }

    [Fact]
    public async Task Should_Validate_Successfully_When_Rating_Is_Valid()
    {
        var dto = new RatingDto(4.5m, 10);

        var result = await _validator.TestValidateAsync(dto);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Should_Fail_When_Rating_Is_Less_Than_Zero()
    {
        var dto = new RatingDto(-1, 10);

        var result = await _validator.TestValidateAsync(dto);

        result.ShouldHaveValidationErrorFor(x => x.Rate)
            .WithErrorMessage("The rating must be between 0 and 5.");
    }

    [Fact]
    public async Task Should_Fail_When_Rating_Is_Greater_Than_Five()
    {
        var dto = new RatingDto(5.1m, 10);

        var result = await _validator.TestValidateAsync(dto);

        result.ShouldHaveValidationErrorFor(x => x.Rate)
            .WithErrorMessage("The rating must be between 0 and 5.");
    }

    [Fact]
    public async Task Should_Fail_When_Count_Is_Negative()
    {
        var dto = new RatingDto(4.5m, -5);

        var result = await _validator.TestValidateAsync(dto);

        result.ShouldHaveValidationErrorFor(x => x.Count)
            .WithErrorMessage("The number of ratings cannot be negative.");
    }

    [Fact]
    public async Task Should_Validate_Successfully_When_Count_Is_Zero()
    {
        var dto = new RatingDto(3.5m, 0);

        var result = await _validator.TestValidateAsync(dto);

        result.ShouldNotHaveAnyValidationErrors();
    }
}