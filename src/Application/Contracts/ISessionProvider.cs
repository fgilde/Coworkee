using System;
using CleanArchitectureBase.Application.Contracts.Services;

namespace CleanArchitectureBase.Application.Contracts
{
    public interface ISessionProvider
    {
        string SessionId { get; }
        string UserIdFromSession { get; }
    }

    public class SimpleSessionProvider : ISessionProvider
    {
        public SimpleSessionProvider(ICurrentUserService userService)
        {
            SessionId = "S_"+(userService?.UserId ?? Guid.NewGuid().ToString());
        }

        public string SessionId { get; }
        public string UserIdFromSession => null;
    }
}
