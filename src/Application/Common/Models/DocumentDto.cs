using System;
using CleanArchitectureBase.Application.Requests;

namespace CleanArchitectureBase.Application.Common.Models
{
    public class DocumentDto: HashableDtoBase
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsPublic { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string URL { get; set; }
        public string DocumentTypeName { get; set; }
        public int DocumentTypeId { get; set; }
        public bool IsBinary { get; set; }
        public string ContentType { get; set; }
        public UploadRequest UploadRequest { get; set; }
    }
}