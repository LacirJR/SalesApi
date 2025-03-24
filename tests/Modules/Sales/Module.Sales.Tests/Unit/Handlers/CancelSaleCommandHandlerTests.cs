using AutoMapper;
using Bogus;
using FluentValidation;
using FluentValidation.Results;
using Module.Sales.Application.Commands.CancelSaleCommand;
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

public class CancelSaleCommandHandlerTests
{
    private readonly ISaleRepository _saleRepository = Substitute.For<ISaleRepository>();
    private readonly ISaleUnitOfWork _unitOfWork = Substitute.For<ISaleUnitOfWork>();
    private readonly IValidator<CancelSaleCommand> _validator = Substitute.For<IValidator<CancelSaleCommand>>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly Faker _faker = new();

    private readonly CancelSaleCommandHandler _handler;

    public CancelSaleCommandHandlerTests()
    {
        _handler = new CancelSaleCommandHandler(
            _saleRepository,
            _unitOfWork,
            _validator,
            _mapper);
    }

    [Fact]
    public async Task Should_Cancel_Sale_When_Valid()
    {
        var sale = new Sale(DateTime.UtcNow, Guid.NewGuid(), _faker.Company.CompanyName(), Guid.NewGuid());
        var command = new CancelSaleCommand(sale.Id);

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>())
            .Returns(sale);

        _mapper.Map<SaleResponseDto>(Arg.Any<Sale>())
            .Returns(new SaleResponseDto());

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.Succeeded);
        await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
        _saleRepository.Received(1).Update(Arg.Is<Sale>(s => s.IsCanceled));
    }

    [Fact]
    public async Task Should_Throw_When_Validation_Fails()
    {
        var command = new CancelSaleCommand(Guid.NewGuid());

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(new[] { new ValidationFailure("SaleId", "Invalid") }));

        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Should_Fail_When_Sale_Not_Found()
    {
        var command = new CancelSaleCommand(Guid.NewGuid());

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>())
            .Returns((Sale?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal(ServiceError.NotFound.Error, result.Error?.Error);
    }

    [Fact]
    public async Task Should_Fail_When_Sale_Already_Canceled_Or_Finalized()
    {
        var sale = new Sale(DateTime.UtcNow, Guid.NewGuid(), _faker.Company.CompanyName(), Guid.NewGuid());
        sale.Cancel();
        var command = new CancelSaleCommand(sale.Id);

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>())
            .Returns(sale);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal("Invalid Operation", result.Error?.Error);
    }
}
