using CleanArchitectureBase.Application.Interfaces.Common;

namespace CleanArchitectureBase.Application.Interfaces.Services
{
    public interface ICurrentUserService : IService
    {
        string UserId { get; }
    }
}