using System;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using lib.Coworkee.Application.Common.Models.Identity;
using lib.Coworkee.Application.Contracts.Services;
using lib.Coworkee.Application.Contracts.Services.Identity;
using Coworkee.Server.Extensions;
using lib.Coworkee.Shared.Constants.Application;
using Microsoft.Extensions.DependencyInjection;
using Nextended.Core.Extensions;
using Nextended.Core.Scopes;
using Coworkee.Application;
using Coworkee.Infrastructure.Models.Identity;
using Coworkee.Infrastructure.Services.Identity;

namespace Coworkee.Server.Services
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
            Claims = Principal?.Claims.AsEnumerable().Select(item => new KeyValuePair<string, string>(item.Type, item.Value)).ToList();
            RoleIds = httpContextAccessor.HttpContext?.Request.Headers[ApplicationConstants.HeaderNames.RoleIdHeader].SelectMany(s => s.Split(",")).ToArray();
        }



        public async Task<IDisposable> AsUser(string userId)
        {
            var user = await _serviceProvider.GetRequiredService<IUserService>().GetAsync(userId);
            if (!user.Succeeded)
                throw Errors.NotFound($"User with id {userId} not found");
            return AsUser(user.Data);
        }

        public async Task<IDisposable> AsSystemUser()
        {
            var systemUser = await _serviceProvider.GetRequiredService<IUserService>().SystemUserAsync();
            return AsUser(systemUser);
        }

        public string UserId { get; private set; }
        public string[] RoleIds { get; private set; }
        public List<KeyValuePair<string, string>> Claims { get; private set; }
        public ClaimsPrincipal Principal { get; private set; }
        public UserResponse CurrentUser() => _serviceProvider.GetService<IUserService>()?.Get(UserId);

        private IDisposable AsUser(UserResponse user)
        {
            return new ActionScope(() =>
            {
                UserId = user.Id;
                Principal = CreateClaimsPrincipalByUser(user).GetAwaiter().GetResult();
            }, () =>
            {
                SetFromHttpContext(_serviceProvider.GetService<IHttpContextAccessor>());
            });
        }

        private async Task<ClaimsPrincipal> CreateClaimsPrincipalByUser(UserResponse user) => new(
            new ClaimsIdentity((await _serviceProvider.GetRequiredService<IdentityService>().GetClaimsAsync(user.MapTo<ApplicationUser>())).ToArray()));
    }

}