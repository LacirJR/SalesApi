using AutoMapper;
using Bogus;
using FluentValidation;
using FluentValidation.Results;
using Module.Sales.Application.Commands.CreateFromCartCommand;
using Module.Sales.Application.Dtos;
using Module.Sales.Application.Interfaces.Persistence;
using Module.Sales.Domain.Entities;
using NSubstitute;
using Shared.Application.Dtos;
using Shared.Application.Interfaces.Persistence;
using Shared.Application.Interfaces.Services;
using Shared.Domain.Common;
using Xunit;
using ValidationException = FluentValidation.ValidationException;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace Module.Sales.Tests.Unit.Handlers;

public class CreateSaleFromCartCommandHandlerTests
{
    private readonly ISaleRepository _saleRepository = Substitute.For<ISaleRepository>();
    private readonly ISharedCartService _sharedCartService = Substitute.For<ISharedCartService>();
    private readonly ISaleUnitOfWork _unitOfWork = Substitute.For<ISaleUnitOfWork>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly IValidator<CreateSaleFromCartCommand> _validator = Substitute.For<IValidator<CreateSaleFromCartCommand>>();
    private readonly Faker _faker = new();
    private readonly CreateSaleFromCartCommandHandler _handler;

    public CreateSaleFromCartCommandHandlerTests()
    {
        _handler = new CreateSaleFromCartCommandHandler(
            _saleRepository,
            _sharedCartService,
            _unitOfWork,
            _mapper,
            _validator
        );
    }

    [Fact]
    public async Task Should_Create_Sale_When_Cart_Is_Valid()
    {
        var cartId = Guid.NewGuid();
        var command = new CreateSaleFromCartCommand(cartId, _faker.Company.CompanyName());

        var cart = new SharedCartDto
        {
            Id = cartId,
            UserId = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            Products = new List<SharedCartItemDto>
            {
                new SharedCartItemDto
                {
                    ProductId = Guid.NewGuid(),
                    Quantity = 2,
                    UnitPrice = 100,
                    DiscountPercentage = 10
                }
            }
        };

        var sale = new Sale(cart.Date, cart.UserId, command.Branch, cart.Id);
        sale.AddItem(new SaleItem(sale.Id, cart.Products[0].ProductId, 2, 100, 10));

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _sharedCartService.GetCartByIdAsync(command.CartId, Arg.Any<CancellationToken>())
            .Returns(cart);

        _mapper.Map<SaleResponseDto>(Arg.Any<Sale>())
            .Returns(new SaleResponseDto());

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.Succeeded);
        await _saleRepository.Received(1).AddAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Fail_When_Validation_Fails()
    {
        var command = new CreateSaleFromCartCommand(Guid.NewGuid(), _faker.Company.CompanyName());

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(new[] { new ValidationFailure("CartId", "Invalid cart id") }));

        await Assert.ThrowsAsync<ValidationException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Should_Fail_When_Cart_Not_Found()
    {
        var command = new CreateSaleFromCartCommand(Guid.NewGuid(), _faker.Company.CompanyName());

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _sharedCartService.GetCartByIdAsync(command.CartId, Arg.Any<CancellationToken>())
            .Returns((SharedCartDto?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal(ServiceError.NotFound.Error, result.Error?.Error);
    }

    [Fact]
    public async Task Should_Fail_When_Cart_Has_No_Products()
    {
        var command = new CreateSaleFromCartCommand(Guid.NewGuid(), _faker.Company.CompanyName());

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _sharedCartService.GetCartByIdAsync(command.CartId, Arg.Any<CancellationToken>())
            .Returns(new SharedCartDto { Products = new List<SharedCartItemDto>() });

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal("Empty cart", result.Error?.Error);
    }
}
