using AutoMapper;
using CleanArchitectureBase.Application.Features.DocumentTypes.Commands.AddEdit;
using CleanArchitectureBase.Application.Features.DocumentTypes.Queries.GetAll;
using CleanArchitectureBase.Application.Features.DocumentTypes.Queries.GetById;
using CleanArchitectureBase.Domain.Entities.Misc;

namespace CleanArchitectureBase.Application.Mappings
{
    public class DocumentTypeProfile : Profile
    {
        public DocumentTypeProfile()
        {
            CreateMap<AddEditDocumentTypeCommand, DocumentType>().ReverseMap();
            CreateMap<GetDocumentTypeByIdResponse, DocumentType>().ReverseMap();
            CreateMap<GetAllDocumentTypesResponse, DocumentType>().ReverseMap();
        }
    }
}