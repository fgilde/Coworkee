using CleanArchitectureBase.Application.Requests;

namespace CleanArchitectureBase.Application.Interfaces.Services
{
    public interface IUploadService
    {
        string UploadAsync(UploadRequest request);
    }
}