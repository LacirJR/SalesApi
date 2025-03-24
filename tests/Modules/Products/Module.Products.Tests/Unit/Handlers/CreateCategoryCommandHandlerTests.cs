using AutoMapper;
using Bogus;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Module.Products.Application.Commands.CreateCategoryCommand;
using Module.Products.Application.Dtos;
using Module.Products.Application.Interfaces.Persistence;
using Module.Products.Domain.Entities;
using NSubstitute;
using Shared.Application.Interfaces.Persistence;
using Xunit;
using ValidationException = FluentValidation.ValidationException;
using ValidationResult = Bogus.ValidationResult;

namespace Module.Products.Tests.Unit.Handlers;

public class CreateCategoryCommandHandlerTests
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductUnitOfWork _unitOfWork;
    private readonly IValidator<CreateCategoryCommand> _validator;
    private readonly IMapper _mapper;
    private readonly CreateCategoryCommandHandler _handler;
    private readonly Faker _faker;
    
    public CreateCategoryCommandHandlerTests()
    {
        _categoryRepository = Substitute.For<ICategoryRepository>();
        _unitOfWork = Substitute.For<IProductUnitOfWork>();
        _validator = Substitute.For<IValidator<CreateCategoryCommand>>();
        _mapper = Substitute.For<IMapper>();

        _handler = new CreateCategoryCommandHandler(_categoryRepository, _unitOfWork, _validator, _mapper);
        _faker = new Faker();
    }
    
    
    [Fact]
    public async Task Should_Create_Category_Successfully()
    {
        var command = new CreateCategoryCommand(_faker.Commerce.Categories(1)[0]);

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new FluentValidation.Results.ValidationResult());

        _mapper.Map<CategoryResponseDto>(Arg.Any<Category>())
            .Returns(new CategoryResponseDto { Name = command.CategoryName });

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.Equal(command.CategoryName, result.Data.Name);

        await _categoryRepository.Received(1).AddAsync(Arg.Any<Category>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task Should_Fail_When_Validation_Fails()
    {
        var command = new CreateCategoryCommand("");

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new FluentValidation.Results.ValidationResult(new[]
            {
                new ValidationFailure("CategoryName", "Category name is required")
            }));

        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));

        await _categoryRepository.DidNotReceive().AddAsync(Arg.Any<Category>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task Should_Fail_When_Category_Already_Exists()
    {
        var existingCategoryName = _faker.Commerce.Categories(1)[0];
        var command = new CreateCategoryCommand(existingCategoryName);

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new FluentValidation.Results.ValidationResult(new[]
            {
                new ValidationFailure("CategoryName", "The specified category already exist.")
            }));

        _categoryRepository.GetByNameAsync(existingCategoryName, Arg.Any<CancellationToken>())
            .Returns(new Category(existingCategoryName)); 

        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));

        await _categoryRepository.DidNotReceive().AddAsync(Arg.Any<Category>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task Should_Fail_When_UnitOfWork_Fails()
    {
        var command = new CreateCategoryCommand(_faker.Commerce.Categories(1)[0]);

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new FluentValidation.Results.ValidationResult());

        _unitOfWork.CommitAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromException<int>(new Exception("Database error")));

        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));

        await _categoryRepository.Received(1).AddAsync(Arg.Any<Category>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }
    
}