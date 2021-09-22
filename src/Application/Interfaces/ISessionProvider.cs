using System;
using CleanArchitectureBase.Application.Interfaces.Services;

namespace CleanArchitectureBase.Application.Interfaces
{
    public interface ISessionProvider
    {
        string SessionId { get; }
    }

    public class SimpleSessionProvider : ISessionProvider
    {
        public SimpleSessionProvider(ICurrentUserService userService)
        {
            SessionId = "S_"+(userService?.UserId ?? Guid.NewGuid().ToString());
        }

        public string SessionId { get; }
    }
}
