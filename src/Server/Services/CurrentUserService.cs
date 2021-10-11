using CleanArchitectureBase.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using CleanArchitectureBase.Shared.Constants.Application;

namespace CleanArchitectureBase.Server.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            UserId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            Claims = httpContextAccessor.HttpContext?.User?.Claims.AsEnumerable().Select(item => new KeyValuePair<string, string>(item.Type, item.Value)).ToList();
            RoleIds = httpContextAccessor.HttpContext?.Request.Headers[ApplicationConstants.HeaderNames.RoleIdHeader].SelectMany(s => s.Split(",")).ToArray();
        }

        public string UserId { get; }
        public string[] RoleIds { get; set; }
        public List<KeyValuePair<string, string>> Claims { get; set; }
    }
}