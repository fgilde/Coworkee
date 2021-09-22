using AutoMapper;
using CleanArchitectureBase.Infrastructure.Models.Identity;
using CleanArchitectureBase.Application.Responses.Identity;

namespace CleanArchitectureBase.Infrastructure.Mappings
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<RoleResponse, BlazorHeroRole>().ReverseMap();
        }
    }
}