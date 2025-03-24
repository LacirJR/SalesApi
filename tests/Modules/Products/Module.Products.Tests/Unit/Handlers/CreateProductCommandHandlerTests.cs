using AutoMapper;
using Bogus;
using FluentValidation;
using FluentValidation.Results;
using Module.Products.Application.Commands.CreateProductCommand;
using Module.Products.Application.Dtos;
using Module.Products.Application.Interfaces.Persistence;
using Module.Products.Domain.Entities;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Shared.Application.Interfaces.Persistence;
using ValidationResult = FluentValidation.Results.ValidationResult;
using ValidationException = FluentValidation.ValidationException;
using Xunit;

namespace Module.Products.Tests.Unit.Handlers;

public class CreateProductCommandHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductUnitOfWork _unitOfWork;
    private readonly IValidator<CreateProductCommand> _validator;
    private readonly IMapper _mapper;
    private readonly CreateProductCommandHandler _handler;
    private readonly Faker _faker;
    
    public CreateProductCommandHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _categoryRepository = Substitute.For<ICategoryRepository>();
        _unitOfWork = Substitute.For<IProductUnitOfWork>();
        _validator = Substitute.For<IValidator<CreateProductCommand>>();
        _mapper = Substitute.For<IMapper>();

        _handler = new CreateProductCommandHandler(_productRepository, _categoryRepository, _unitOfWork, _validator, _mapper);
        _faker = new Faker();
    }
    
    [Fact]
    public async Task Should_Create_Product_Successfully()
    {
        var category = new Category(_faker.Commerce.Categories(1)[0]);
        var command = new CreateProductCommand(
            _faker.Commerce.ProductName(),
            _faker.Random.Decimal(1, 100),
            _faker.Lorem.Sentence(),
            category.Name,
            _faker.Image.PicsumUrl(),
            new RatingDto(4.5m, 10)
        );

        _validator.ValidateAsync(command, default)
            .Returns(new ValidationResult());

        _categoryRepository.GetByNameAsync(command.Category, default)
            .Returns(Task.FromResult(category));

        _mapper.Map<ProductResponseDto>(Arg.Any<Product>())
            .Returns(new ProductResponseDto { Title = command.Title, Price = command.Price });

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.Equal(command.Title, result.Data.Title);

        await _productRepository.Received(1).AddAsync(Arg.Any<Product>(), default);
        await _unitOfWork.Received(1).CommitAsync(default);
    }
    
    [Fact]
    public async Task Should_Fail_When_Validation_Fails()
    {
        var command = new CreateProductCommand("", 0, "", "", "", new RatingDto(0, 0));

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(new[]
            {
                new ValidationFailure("Title", "Title is required"),
                new ValidationFailure("Price", "Price must be greater than zero")
            }));

        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));

        await _productRepository.DidNotReceive().AddAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task Should_Fail_When_Category_Not_Found()
    {
        var command = new CreateProductCommand(
            _faker.Commerce.ProductName(),
            _faker.Random.Decimal(1, 100),
            _faker.Lorem.Sentence(),
            "NonExistentCategory",
            _faker.Image.PicsumUrl(),
            new RatingDto(4.5m, 10)
        );

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _categoryRepository.GetByNameAsync(command.Category, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Category>(null)); 

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal("NotFound", result.Error.Type);
        Assert.Equal("Category not found.", result.Error.Detail);

        await _productRepository.DidNotReceive().AddAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task Should_Fail_When_UnitOfWork_Fails()
    {
        var category = new Category(_faker.Commerce.Categories(1)[0]);
        var command = new CreateProductCommand(
            _faker.Commerce.ProductName(),
            _faker.Random.Decimal(1, 100),
            _faker.Lorem.Sentence(),
            category.Name,
            _faker.Image.PicsumUrl(),
            new RatingDto(4.5m, 10)
        );

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _categoryRepository.GetByNameAsync(command.Category, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(category));

        _unitOfWork.CommitAsync(Arg.Any<CancellationToken>())
            .ThrowsAsync(new Exception("Database error"));
        
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));

        await _productRepository.Received(1).AddAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }
    
}