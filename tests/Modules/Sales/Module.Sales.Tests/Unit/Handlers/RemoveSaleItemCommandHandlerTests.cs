using AutoMapper;
using Bogus;
using FluentValidation;
using FluentValidation.Results;
using Module.Sales.Application.Commands.RemoveSaleItemCommand;
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

public class RemoveSaleItemCommandHandlerTests
{
    private readonly ISaleRepository _saleRepository = Substitute.For<ISaleRepository>();
    private readonly ISaleUnitOfWork _unitOfWork = Substitute.For<ISaleUnitOfWork>();
    private readonly IValidator<RemoveSaleItemCommand> _validator = Substitute.For<IValidator<RemoveSaleItemCommand>>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly RemoveSaleItemCommandHandler _handler;
    private readonly Faker _faker = new();

    public RemoveSaleItemCommandHandlerTests()
    {
        _handler = new RemoveSaleItemCommandHandler(_saleRepository, _unitOfWork, _validator, _mapper);
    }

    [Fact]
    public async Task Should_Remove_Item_Successfully()
    {
        var command = new RemoveSaleItemCommand(Guid.NewGuid(), Guid.NewGuid());
        var sale = new Sale(DateTime.UtcNow, Guid.NewGuid(), _faker.Company.CompanyName(), Guid.NewGuid());
        var item = new SaleItem(sale.Id, command.ProductId, 2, 100, 10);
        sale.AddItem(item);

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>())
            .Returns(sale);

        var expectedDto = new SaleResponseDto();
        _mapper.Map<SaleResponseDto>(Arg.Any<Sale>()).Returns(expectedDto);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.Equal(expectedDto, result.Data);
        await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Throw_ValidationException_When_Validation_Fails()
    {
        var command = new RemoveSaleItemCommand(Guid.NewGuid(), Guid.NewGuid());

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(new[] { new ValidationFailure("SaleId", "Invalid") }));

        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Should_Return_NotFound_When_Sale_Does_Not_Exist()
    {
        var command = new RemoveSaleItemCommand(Guid.NewGuid(), Guid.NewGuid());

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>())
            .Returns((Sale?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal(ServiceError.NotFound, result.Error);
    }

    [Fact]
    public async Task Should_Return_Error_When_Sale_Is_Finalized_Or_Canceled()
    {
        var command = new RemoveSaleItemCommand(Guid.NewGuid(), Guid.NewGuid());
        var sale = new Sale(DateTime.UtcNow, Guid.NewGuid(), _faker.Company.CompanyName(), Guid.NewGuid());
        sale.Finish();

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>())
            .Returns(sale);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal("Validation", result.Error.Type);
        Assert.Equal("Cannot remove items from a canceled or finalized sale.", result.Error.Detail);
    }
}
