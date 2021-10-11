using System.Collections.Generic;
using CleanArchitectureBase.Application.Interfaces.Common;

namespace CleanArchitectureBase.Application.Interfaces.Services
{
    public interface ICurrentUserService : IService
    {
        string UserId { get; }
        string[] RoleIds { get; }
        List<KeyValuePair<string, string>> Claims { get; }
    }
}