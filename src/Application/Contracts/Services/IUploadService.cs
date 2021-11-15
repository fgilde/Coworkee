using CleanArchitectureBase.Application.Requests;

namespace CleanArchitectureBase.Application.Contracts.Services
{
    public interface IUploadService
    {
        string UploadAsync(UploadRequest request);
    }
}