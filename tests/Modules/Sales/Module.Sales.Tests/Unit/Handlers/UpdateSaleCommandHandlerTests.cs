using AutoMapper;
using Bogus;
using FluentValidation;
using FluentValidation.Results;
using Module.Sales.Application.Commands.UpdateSaleCommand;
using Module.Sales.Application.Dtos;
using Module.Sales.Application.Interfaces.Persistence;
using Module.Sales.Domain.Entities;
using NSubstitute;
using Shared.Application.Interfaces.Persistence;
using Shared.Domain.Common;
using Xunit;
using ValidationException = FluentValidation.ValidationException;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace Module.Sales.Tests.Unit.Handlers;

public class UpdateSaleCommandHandlerTests
{
    private readonly ISaleRepository _saleRepository = Substitute.For<ISaleRepository>();
    private readonly IValidator<UpdateSaleCommand> _validator = Substitute.For<IValidator<UpdateSaleCommand>>();
    private readonly ISaleUnitOfWork _unitOfWork = Substitute.For<ISaleUnitOfWork>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly UpdateSaleCommandHandler _handler;
    private readonly Faker _faker = new();

    public UpdateSaleCommandHandlerTests()
    {
        _handler = new UpdateSaleCommandHandler(_saleRepository, _validator, _unitOfWork, _mapper);
    }

    [Fact]
    public async Task Should_Update_Sale_Successfully()
    {
        var command = new UpdateSaleCommand(Guid.NewGuid(), _faker.Company.CompanyName(), _faker.Date.Past());
        var sale = new Sale(DateTime.UtcNow, Guid.NewGuid(), "Old Branch", Guid.NewGuid());

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>())
            .Returns(sale);

        var expected = new SaleResponseDto();
        _mapper.Map<SaleResponseDto>(sale).Returns(expected);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.Equal(expected, result.Data);
        await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Throw_ValidationException_When_Validation_Fails()
    {
        var command = new UpdateSaleCommand(Guid.NewGuid(), _faker.Company.CompanyName(), _faker.Date.Past());

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(new[] { new ValidationFailure("Branch", "Invalid") }));

        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Should_Return_NotFound_When_Sale_Does_Not_Exist()
    {
        var command = new UpdateSaleCommand(Guid.NewGuid(), _faker.Company.CompanyName(), _faker.Date.Past());

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>())
            .Returns((Sale?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal(ServiceError.NotFound, result.Error);
    }

    [Fact]
    public async Task Should_Return_Error_When_Sale_Is_Canceled()
    {
        var command = new UpdateSaleCommand(Guid.NewGuid(), _faker.Company.CompanyName(), _faker.Date.Past());
        var sale = new Sale(DateTime.UtcNow, Guid.NewGuid(), "Branch", Guid.NewGuid());
        sale.Cancel();

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>())
            .Returns(sale);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal("Validation", result.Error.Type);
        Assert.Equal("Cannot update a canceled sale.", result.Error.Detail);
    }
}
