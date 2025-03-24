using AutoMapper;
using Bogus;
using FluentValidation;
using FluentValidation.Results;
using Module.Sales.Application.Commands.FinishSaleCommand;
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

public class FinishSaleCommandHandlerTests
{
    private readonly ISaleRepository _saleRepository = Substitute.For<ISaleRepository>();
    private readonly ISaleUnitOfWork _unitOfWork = Substitute.For<ISaleUnitOfWork>();
    private readonly IValidator<FinishSaleCommand> _validator = Substitute.For<IValidator<FinishSaleCommand>>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly FinishSaleCommandHandler _handler;
    private readonly Faker _faker = new();

    public FinishSaleCommandHandlerTests()
    {
        _handler = new FinishSaleCommandHandler(_saleRepository, _unitOfWork, _validator, _mapper);
    }

    [Fact]
    public async Task Should_Finish_Sale_Successfully()
    {
        var command = new FinishSaleCommand(Guid.NewGuid());
        var sale = new Sale(DateTime.UtcNow, Guid.NewGuid(), _faker.Company.CompanyName(), Guid.NewGuid());
        var expectedDto = new SaleResponseDto();

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>())
            .Returns(sale);

        _mapper.Map<SaleResponseDto>(Arg.Any<Sale>()).Returns(expectedDto);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.Equal(expectedDto, result.Data);
        await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Fail_When_Validation_Fails()
    {
        var command = new FinishSaleCommand(Guid.NewGuid());

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(new[] { new ValidationFailure("SaleId", "Invalid") }));

        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Should_Return_NotFound_When_Sale_Not_Exists()
    {
        var command = new FinishSaleCommand(Guid.NewGuid());

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>())
            .Returns((Sale?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal(ServiceError.NotFound, result.Error);
    }

    [Fact]
    public async Task Should_Fail_When_Sale_Already_Canceled_Or_Finalized()
    {
        var command = new FinishSaleCommand(Guid.NewGuid());
        var sale = new Sale(DateTime.UtcNow, Guid.NewGuid(), _faker.Company.CompanyName(), Guid.NewGuid());
        sale.Finish(); 

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>())
            .Returns(sale);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal("Validation", result.Error.Type);
        Assert.Equal("Sale is already finalized or canceled.", result.Error.Detail);
    }
}
