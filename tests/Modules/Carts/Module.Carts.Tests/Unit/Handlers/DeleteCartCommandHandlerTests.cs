using Bogus;
using FluentValidation;
using FluentValidation.Results;
using Module.Carts.Application.Commands.DeleteCartCommand;
using Module.Carts.Application.Dtos;
using Module.Carts.Application.Interfaces.Persistence;
using Module.Carts.Domain.Entities;
using NSubstitute;
using ValidationResult = FluentValidation.Results.ValidationResult;
using ValidationException = FluentValidation.ValidationException;
using Shared.Application.Interfaces.Persistence;
using Shared.Domain.Common;
using Xunit;

namespace Module.Carts.Tests.Unit.Handlers;

public class DeleteCartCommandHandlerTests
{
    private readonly Faker _faker;
    private readonly ICartRepository _cartRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<DeleteCartCommand> _validator;
    private readonly DeleteCartCommandHandler _handler;

    public DeleteCartCommandHandlerTests()
    {
        _faker = new Faker();
        _cartRepository = Substitute.For<ICartRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _validator = Substitute.For<IValidator<DeleteCartCommand>>();

        _handler = new DeleteCartCommandHandler(_cartRepository, _unitOfWork, _validator);
    }
    
    [Fact]
    public async Task Should_Throw_ValidationException_When_Command_Is_Invalid()
    {
        var command = new DeleteCartCommand(Guid.Empty);

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(new[] {
                new ValidationFailure("CartId", "CartId is required.")
            }));

        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
    }
    
    [Fact]
    public async Task Should_Fail_When_Cart_Not_Found()
    {
        var command = new DeleteCartCommand(_faker.Random.Guid());

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _cartRepository.GetByIdAsync(command.CartId, Arg.Any<CancellationToken>())
            .Returns((Cart?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal(ServiceError.NotFound.Type, result.Error?.Type);
    }
    
    [Fact]
    public async Task Should_Delete_Cart_Successfully()
    {
        var cartId = _faker.Random.Guid();
        var command = new DeleteCartCommand(cartId);
        var cart = new Cart(_faker.Random.Guid(), _faker.Date.Past());

        typeof(BaseEntity).GetProperty(nameof(BaseEntity.Id))!
            .SetValue(cart, cartId);

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new ValidationResult()));

        _cartRepository.GetByIdAsync(cartId, Arg.Any<CancellationToken>())
            .Returns(cart);

        var result = await _handler.Handle(command, CancellationToken.None);

        await _cartRepository.Received(1).RemoveByIdAsync(cartId, Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());

        Assert.True(result.Succeeded);
        Assert.Equal(new DeleteCartDto("Successfully deleted cart"), result.Data);
    }
    
}