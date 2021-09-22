using AutoMapper;
using CleanArchitectureBase.Infrastructure.Models.Identity;
using CleanArchitectureBase.Application.Responses.Identity;

namespace CleanArchitectureBase.Infrastructure.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserResponse, BlazorHeroUser>().ReverseMap();
            CreateMap<ChatUserResponse, BlazorHeroUser>().ReverseMap()
                .ForMember(dest => dest.EmailAddress, source => source.MapFrom(source => source.Email)); //Specific Mapping
        }
    }
}