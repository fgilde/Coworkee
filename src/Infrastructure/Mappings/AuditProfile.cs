using AutoMapper;
using CleanArchitectureBase.Infrastructure.Models.Audit;
using CleanArchitectureBase.Application.Responses.Audit;

namespace CleanArchitectureBase.Infrastructure.Mappings
{
    public class AuditProfile : Profile
    {
        public AuditProfile()
        {
            CreateMap<AuditResponse, Audit>().ReverseMap();
        }
    }
}