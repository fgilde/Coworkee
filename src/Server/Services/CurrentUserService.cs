using System;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using CleanArchitectureBase.Application.Common.Models.Identity;
using CleanArchitectureBase.Application.Contracts.Services;
using CleanArchitectureBase.Application.Contracts.Services.Identity;
using CleanArchitectureBase.Server.Extensions;
using CleanArchitectureBase.Shared.Constants.Application;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitectureBase.Server.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IServiceProvider _serviceProvider;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            UserId = httpContextAccessor.HttpContext.GetUserId();
            Principal = httpContextAccessor.HttpContext?.User;
            Claims = httpContextAccessor.HttpContext?.User?.Claims.AsEnumerable().Select(item => new KeyValuePair<string, string>(item.Type, item.Value)).ToList();
            RoleIds = httpContextAccessor.HttpContext?.Request.Headers[ApplicationConstants.HeaderNames.RoleIdHeader].SelectMany(s => s.Split(",")).ToArray();
        }

        public string UserId { get; }
        public string[] RoleIds { get; }
        public List<KeyValuePair<string, string>> Claims { get; }
        public ClaimsPrincipal Principal { get; }
        public UserResponse CurrentUser() => _serviceProvider.GetService<IUserService>()?.Get(UserId);
    }
}