//using AutoMapper;
//using CleanArchitectureBase.Application.Requests.Identity;
//using CleanArchitectureBase.Application.Responses.Identity;
//using CleanArchitectureBase.Infrastructure.Models.Identity;

//namespace CleanArchitectureBase.Infrastructure.Mappings
//{
//    public class RoleClaimProfile : Profile
//    {
//        public RoleClaimProfile()
//        {
//            CreateMap<RoleClaimResponse, BlazorHeroRoleClaim>()
//                .ForMember(nameof(BlazorHeroRoleClaim.ClaimType), opt => opt.MapFrom(c => c.Type))
//                .ForMember(nameof(BlazorHeroRoleClaim.ClaimValue), opt => opt.MapFrom(c => c.Value))
//                .ReverseMap();

//            CreateMap<RoleClaimRequest, BlazorHeroRoleClaim>()
//                .ForMember(nameof(BlazorHeroRoleClaim.ClaimType), opt => opt.MapFrom(c => c.Type))
//                .ForMember(nameof(BlazorHeroRoleClaim.ClaimValue), opt => opt.MapFrom(c => c.Value))
//                .ReverseMap();
//        }
//    }
//}