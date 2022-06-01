using CleanArchitectureBase.Application.Contracts.Enums;

namespace CleanArchitectureBase.Application.Requests
{
    public class UploadRequest
    {
        public string FileName { get; set; }
        public string Extension { get; set; }
        public string ContentType { get; set; }
        public UploadType UploadType { get; set; }
        public byte[] Data { get; set; }
    }
}