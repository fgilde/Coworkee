using System;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Common.Models.Identity;
using CleanArchitectureBase.Application.Contracts.Services;
using CleanArchitectureBase.Application.Contracts.Services.Identity;
using CleanArchitectureBase.Server.Extensions;
using CleanArchitectureBase.Shared.Constants.Application;
using Microsoft.Extensions.DependencyInjection;
using Nextended.Core.Scopes;

namespace CleanArchitectureBase.Server.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IServiceProvider _serviceProvider;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            SetFromHttpContext(httpContextAccessor);
        }

        private void SetFromHttpContext(IHttpContextAccessor httpContextAccessor)
        {
            UserId = httpContextAccessor.HttpContext.GetUserId();
            Principal = httpContextAccessor.HttpContext?.User;
            Claims = httpContextAccessor.HttpContext?.User?.Claims.AsEnumerable().Select(item => new KeyValuePair<string, string>(item.Type, item.Value)).ToList();
            RoleIds = httpContextAccessor.HttpContext?.Request.Headers[ApplicationConstants.HeaderNames.RoleIdHeader].SelectMany(s => s.Split(",")).ToArray();
        }

        public async Task<IDisposable> AsSystemUser()
        {
            var systemUser = await _serviceProvider.GetService<IUserService>().SystemUserAsync();
            return new ActionScope(() =>
            {
                UserId = systemUser.Id;
            }, () =>
            {
                SetFromHttpContext(_serviceProvider.GetService<IHttpContextAccessor>());
            });
        }

        public string UserId { get; private set; }
        public string[] RoleIds { get; private set; }
        public List<KeyValuePair<string, string>> Claims { get; private set; }
        public ClaimsPrincipal Principal { get; private set; }
        public UserResponse CurrentUser() => _serviceProvider.GetService<IUserService>()?.Get(UserId);
    }

}