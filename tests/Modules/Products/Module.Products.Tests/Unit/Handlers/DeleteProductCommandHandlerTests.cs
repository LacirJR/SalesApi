using AutoMapper;
using Bogus;
using FluentValidation;
using FluentValidation.Results;
using Module.Products.Application.Commands.DeleteProductCommand;
using Module.Products.Application.Dtos;
using Module.Products.Application.Interfaces.Persistence;
using Module.Products.Domain.Entities;
using Module.Products.Domain.ValueObjects;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Shared.Application.Interfaces.Persistence;
using Shared.Domain.Common;
using ValidationResult = FluentValidation.Results.ValidationResult;
using ValidationException = FluentValidation.ValidationException;
using Xunit;

namespace Module.Products.Tests.Unit.Handlers;

public class DeleteProductCommandHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<DeleteProductCommand> _validator;
    private readonly IMapper _mapper;
    private readonly DeleteProductCommandHandler _handler;
    private readonly Faker _faker;

    public DeleteProductCommandHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _validator = Substitute.For<IValidator<DeleteProductCommand>>();
        _mapper = Substitute.For<IMapper>();

        _handler = new DeleteProductCommandHandler(_productRepository, _unitOfWork, _validator, _mapper);
        _faker = new Faker();
    }
    
    [Fact]
    public async Task Should_Delete_Product_Successfully()
    {
        var productId = Guid.NewGuid();
        var product = new Product(
            _faker.Commerce.ProductName(),
            _faker.Random.Decimal(1, 100),
            _faker.Lorem.Sentence(),
            Guid.NewGuid(),
            _faker.Image.PicsumUrl(),
            new Rating(4.5m, 10)
        );
        
        var command = new DeleteProductCommand(productId);

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _productRepository.RemoveByIdAsync(productId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(product));

        _mapper.Map<ProductResponseDto>(Arg.Any<Product>())
            .Returns(new ProductResponseDto { Title = product.Title, Price = product.Price });

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.Equal(product.Title, result.Data.Title);

        await _productRepository.Received(1).RemoveByIdAsync(productId, Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task Should_Fail_When_Validation_Fails()
    {
        var command = new DeleteProductCommand(Guid.Empty);

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(new[]
            {
                new ValidationFailure("Id", "Product ID is required")
            }));

        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));

        await _productRepository.DidNotReceive().RemoveByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task Should_Fail_When_Product_Not_Found()
    {
        var productId = Guid.NewGuid();
        var command = new DeleteProductCommand(productId);

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _productRepository.RemoveByIdAsync(productId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Product>(null));

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal(ServiceError.NotFound, result.Error);

        await _productRepository.Received(1).RemoveByIdAsync(productId, Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task Should_Fail_When_UnitOfWork_Fails()
    {
        var productId = Guid.NewGuid();
        var product = new Product(
            _faker.Commerce.ProductName(),
            _faker.Random.Decimal(1, 100),
            _faker.Lorem.Sentence(),
            Guid.NewGuid(),
            _faker.Image.PicsumUrl(),
            new Rating(4.5m, 10)
        );

        var command = new DeleteProductCommand(productId);

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _productRepository.RemoveByIdAsync(productId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(product));

        _unitOfWork.CommitAsync(Arg.Any<CancellationToken>())
            .ThrowsAsync(new Exception("Database error"));

        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));

        await _productRepository.Received(1).RemoveByIdAsync(productId, Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }
    
}