using AutoMapper;
using CleanArchitectureBase.Application.Requests.Identity;
using CleanArchitectureBase.Application.Responses.Identity;

namespace CleanArchitectureBase.Client.Infrastructure.Mappings
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<PermissionResponse, PermissionRequest>().ReverseMap();
            CreateMap<RoleClaimResponse, RoleClaimRequest>().ReverseMap();
        }
    }
}