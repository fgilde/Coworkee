using AutoMapper;
using CleanArchitectureBase.Application.Features.Documents.Commands.AddEdit;
using CleanArchitectureBase.Application.Features.Documents.Queries.GetById;
using CleanArchitectureBase.Domain.Entities.Misc;

namespace CleanArchitectureBase.Application.Mappings
{
    public class DocumentProfile : Profile
    {
        public DocumentProfile()
        {
            CreateMap<AddEditDocumentCommand, Document>().ReverseMap();
            CreateMap<GetDocumentByIdResponse, Document>().ReverseMap();
        }
    }
}