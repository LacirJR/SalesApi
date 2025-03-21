using AutoMapper;
using Bogus;
using FluentValidation;
using FluentValidation.Results;
using Module.Products.Application.Commands.DeleteCategoryCommand;
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

public class DeleteCategoryCommandHandlerTests
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<DeleteCategoryCommand> _validator;
    private readonly IMapper _mapper;
    private readonly DeleteCategoryCommandHandler _handler;
    private readonly Faker _faker;

    public DeleteCategoryCommandHandlerTests()
    {
        _categoryRepository = Substitute.For<ICategoryRepository>();
        _productRepository = Substitute.For<IProductRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _validator = Substitute.For<IValidator<DeleteCategoryCommand>>();
        _mapper = Substitute.For<IMapper>();

        _handler = new DeleteCategoryCommandHandler(_categoryRepository, _productRepository, _unitOfWork, _validator, _mapper);
        _faker = new Faker();
    }
    
    [Fact]
    public async Task Should_Delete_Category_Successfully()
    {
        var categoryName = _faker.Commerce.Categories(1)[0];
        var category = new Category(categoryName);
        var command = new DeleteCategoryCommand(categoryName);

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _categoryRepository.GetByNameAsync(categoryName, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(category));

        _productRepository.AnyByCategoryIdAsync(category.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(false));

        _categoryRepository.RemoveByNameAsync(categoryName, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(category));

        _mapper.Map<CategoryResponseDto>(Arg.Any<Category>())
            .Returns(new CategoryResponseDto { Name = categoryName });

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.Equal(categoryName, result.Data.Name);

        await _categoryRepository.Received(1).RemoveByNameAsync(categoryName, Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task Should_Fail_When_Validation_Fails()
    {
        var command = new DeleteCategoryCommand("");

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(new[]
            {
                new ValidationFailure("CategoryName", "Category name is required")
            }));

        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));

        await _categoryRepository.DidNotReceive().RemoveByNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task Should_Fail_When_Category_Not_Found()
    {
        var command = new DeleteCategoryCommand("NonExistentCategory");

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _categoryRepository.GetByNameAsync(command.CategoryName, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Category>(null));

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal("NotFound", result.Error.Type);
        Assert.Equal("Category not found.", result.Error.Detail);

        await _categoryRepository.DidNotReceive().RemoveByNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task Should_Fail_When_Category_Has_Products()
    {
        var category = new Category(_faker.Commerce.Categories(1)[0]);
        var command = new DeleteCategoryCommand(category.Name);

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _categoryRepository.GetByNameAsync(command.CategoryName, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(category));

        _productRepository.AnyByCategoryIdAsync(category.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(true)); // Categoria tem produtos

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal("Validation", result.Error.Type);
        Assert.Equal("Cannot delete the category because there are products associated with it.", result.Error.Detail);

        await _categoryRepository.DidNotReceive().RemoveByNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task Should_Fail_When_UnitOfWork_Fails()
    {
        var category = new Category(_faker.Commerce.Categories(1)[0]);
        var command = new DeleteCategoryCommand(category.Name);

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _categoryRepository.GetByNameAsync(command.CategoryName, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(category));

        _productRepository.AnyByCategoryIdAsync(category.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(false));

        _categoryRepository.RemoveByNameAsync(category.Name, Arg.Any<CancellationToken>()).Returns(Task.FromResult(category));
        
        _unitOfWork.CommitAsync(Arg.Any<CancellationToken>())
            .ThrowsAsync(new Exception("Database error"));

        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));

        await _categoryRepository.Received(1).RemoveByNameAsync(category.Name, Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }
    
}