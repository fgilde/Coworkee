using System;
using CleanArchitectureBase.Application.Interfaces;
using CleanArchitectureBase.Shared.Constants.Application;
using Microsoft.AspNetCore.Http;

namespace CleanArchitectureBase.Server
{
    public class SessionProvider : ISessionProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        public SessionProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            SessionId = httpContextAccessor?.HttpContext?.Session.GetString(ApplicationConstants.SessionIdKey) ?? Guid.NewGuid().ToString();
        }

        public string SessionId { get; set; }
    }
}
