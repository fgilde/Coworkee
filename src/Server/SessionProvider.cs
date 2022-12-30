using System;
using Coworkee.Application.Contracts;
using Coworkee.Shared.Constants.Application;
using Microsoft.AspNetCore.Http;

namespace Coworkee.Server
{
    public class SessionProvider : ISessionProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        public SessionProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            SessionId = httpContextAccessor?.HttpContext?.Session.GetString(ApplicationConstants.SessionIdKey) ?? Guid.NewGuid().ToString();
            UserIdFromSession = httpContextAccessor?.HttpContext?.Session.GetString(ApplicationConstants.Session.SessionUserIdKey);
        }

        public string SessionId { get; set; }
        public string UserIdFromSession { get; set; }
    }
}
