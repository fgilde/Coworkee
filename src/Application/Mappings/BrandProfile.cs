using AutoMapper;
using CleanArchitectureBase.Application.Features.Brands.Commands.AddEdit;
using CleanArchitectureBase.Application.Features.Brands.Queries.GetAll;
using CleanArchitectureBase.Application.Features.Brands.Queries.GetById;
using CleanArchitectureBase.Domain.Entities.Catalog;

namespace CleanArchitectureBase.Application.Mappings
{
    public class BrandProfile : Profile
    {
        public BrandProfile()
        {
            CreateMap<AddEditBrandCommand, Brand>().ReverseMap();
            CreateMap<GetBrandByIdResponse, Brand>().ReverseMap();
            CreateMap<GetAllBrandsResponse, Brand>().ReverseMap();
        }
    }
}