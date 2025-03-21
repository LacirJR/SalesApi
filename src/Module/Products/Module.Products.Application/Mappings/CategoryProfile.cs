using AutoMapper;
using Module.Products.Application.Commands.CreateCategoryCommand;
using Module.Products.Application.Dtos;
using Module.Products.Domain.Entities;

namespace Module.Products.Application.Mappings;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, CategoryResponseDto>();
    }
}