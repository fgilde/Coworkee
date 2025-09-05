using lib.Coworkee.Application.Configurations;
using Coworkee.Infrastructure.Models.Identity;
using lib.Coworkee.Application.Requests.Identity;
using lib.Coworkee.Shared.Wrapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using lib.Coworkee.Application.Common.Models.Identity;
using lib.Coworkee.Application.Contracts.Services.Identity;
using lib.Coworkee.Shared.Constants.Application;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Nextended.Core.Extensions;
using lib.Coworkee.Application.Common.Extensions;
using lib.Coworkee.Application.Contracts.Services;
using lib.Coworkee.Application.Hubs.Events;
using lib.Coworkee.Domain.Entities.Identity;
using Coworkee.Infrastructure.Extensions;
using lib.Coworkee.Application.Validators.Requests.Identity;
using lib.Coworkee.Shared.Constants.Role;
using Nextended.Core.Attributes;

namespace Coworkee.Infrastructure.Services.Identity
{
    [RegisterAs(typeof(ITokenService), 1, RegisterAsImplementation = true, ServiceLifetime = ServiceLifetime.Transient)]
    public class IdentityService : ITokenService
    {
        private const string InvalidErrorMessage = "Invalid email or password.";

        private readonly IServiceProvider _serviceProvider;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ServerConfiguration _appConfig;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IStringLocalizer<IdentityService> _localizer;

        public IdentityService(
            IServiceProvider serviceProvider,
            UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager,
            IOptions<ServerConfiguration> appConfig, SignInManager<ApplicationUser> signInManager,
            IStringLocalizer<IdentityService> localizer, IHttpContextAccessor contextAccessor)
        {
            _serviceProvider = serviceProvider;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _appConfig = appConfig.Value;
            _localizer = localizer;
            _contextAccessor = contextAccessor;
        }


        public async Task<Result<TokenResponse>> LoginAsync(ApplicationUser user)
        {
            var allowedEmails = _appConfig.PublicSettings?.LoginSettings?.AllowedEmails;
            var allowedToLogin = allowedEmails == null || allowedEmails.Count == 0 || allowedEmails.Any(pattern => RegisterRequestValidator.MatchesPattern(user.Email, pattern));
            var roles = await _userManager.GetRolesAsync(user);
            if (!allowedToLogin && roles?.Contains(RoleConstants.AdministratorRole) != true)
                return await Result<TokenResponse>.FailAsync(_localizer["Email is not allowed."]);

            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(ApplicationConstants.Session.RefreshTokenExpiryInDays);
            user.UserInfo ??= new UserInformations();
            user.UserInfo.LastLoginDate = DateTime.UtcNow;
            user.UserInfo.IsOnline = true;
            await _userManager.UpdateAsync(user);
            _ = _serviceProvider.GetService<IMediator>().PublishClientEvent(new UserOnlineStatusChanged(user.MapTo<UserResponse>()));

            var token = await GenerateJwtAsync(user, null);
            var response = new TokenResponse { Token = token, RefreshToken = user.RefreshToken, UserImageURL = user.ProfilePictureDataUrl };
            _contextAccessor.HttpContext?.Session?.SetString(ApplicationConstants.Session.SessionUserIdKey, user.Id);

            //_= SetUserOnlineStatusAsync(user, true)
            return await Result<TokenResponse>.SuccessAsync(response);
        }

        public async Task<Result<TokenResponse>> LoginExternalAsync(ClaimsPrincipal externalClaim, ExternalLoginOptions options)
        {
            var claims = externalClaim?.Claims?.ToArray();
            if (claims == null || !claims.Any())
                return Result<TokenResponse>.Fail(_localizer["Invalid external login."]);
            
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var userName = claims?.FirstOrDefault(c => c.Type == "preferred_username")?.Value ?? claims?.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;

            ApplicationUser user = null;
            if (!string.IsNullOrWhiteSpace(email))
                user = await _userManager.FindByEmailAsync(email);
            if (user == null && !string.IsNullOrEmpty(userName))
                user = await _userManager.FindByNameAsync(userName);

            if (user == null)
            {
                if(!options.RegisterIfNotExists)
                    return Result<TokenResponse>.Fail(_localizer["User not found."]);
                var userResponse = (await _serviceProvider.GetService<IUserService>().GetOrAddUserAsync(externalClaim))?.FirstOrDefault();
                if (userResponse != null)
                    user = await _userManager.FindByIdAsync(userResponse.Id);
            }
            if (user == null)
                return Result<TokenResponse>.Fail(_localizer["User not found."]);

            if(!string.IsNullOrWhiteSpace(options.Token))
            {
                var dictionaryService = _serviceProvider.GetService<Application.Contracts.Services.Identity.IDictionaryService>();
                await dictionaryService.SetAsync($"{options.Scheme}_token_for_{user.Id}", options.Token);
            }

            return await LoginAsync(user);
        }

        public async Task<Result<TokenResponse>> LoginAsync(TokenRequest model)
        {
            var user = await _userManager.FindByEmailFullyLoadedAsync(model.Email);
            
            if(user == null && _serviceProvider.GetService<ServerConfiguration>()?.PublicSettings?.LoginSettings?.AllowLoginWithUsername == true)
            {
                user = await _userManager.FindByLoginNameFullyLoadedAsync(model.Email);
            }

            if (user == null)
            {
                return await Result<TokenResponse>.FailAsync(_localizer["User Not Found."]);
            }
            if (!user.EmailConfirmed)
            {
                return await Result<TokenResponse>.FailAsync(_localizer["E-Mail not confirmed."]);
            }
            if (!user.IsActive)
            {
                return await Result<TokenResponse>.FailAsync(_localizer["User Not Active. Please contact the administrator."]);
            }
            var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!passwordValid)
            {
                return await Result<TokenResponse>.FailAsync(_localizer["Invalid Credentials."]);
            }

            return await LoginAsync(user);
        }

        public async Task<Result<TokenResponse>> RegenerateTokenAsync(string[] specificRoles)
        {
            var user = await _userManager.FindByIdAsync(_serviceProvider.GetRequiredService<ICurrentUserService>().UserId);
            if (user == null)
                return await Result<TokenResponse>.FailAsync(_localizer["User Not Found."]);
            var token = await GenerateJwtAsync(user, specificRoles);
            var response = new TokenResponse { Token = token, RefreshToken = user.RefreshToken, UserImageURL = user.ProfilePictureDataUrl };
            return await Result<TokenResponse>.SuccessAsync(response);
        }

        public async Task<Result<TokenResponse>> GetRefreshTokenAsync(RefreshTokenRequest model)
        {
            if (model is null)
            {
                return await Result<TokenResponse>.FailAsync(_localizer["Invalid Client Token."]);
            }
            var userPrincipal = GetPrincipalFromExpiredToken(model.Token);
            var userEmail = userPrincipal.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
                return await Result<TokenResponse>.FailAsync(_localizer["User Not Found."]);
            if (user.RefreshToken != model.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                return await Result<TokenResponse>.FailAsync(_localizer["Invalid Client Token."]);
            var token = GenerateEncryptedToken(GetSigningCredentials(), await GetClaimsAsync(user, null));
            user.RefreshToken = GenerateRefreshToken();
            await _userManager.UpdateAsync(user);

            var response = new TokenResponse { Token = token, RefreshToken = user.RefreshToken, RefreshTokenExpiryTime = user.RefreshTokenExpiryTime };
            return await Result<TokenResponse>.SuccessAsync(response);
        }

        public async Task<string> GenerateTokenForUser(string userId)
        {
            var user = !string.IsNullOrWhiteSpace(userId) ? await _userManager.FindByIdAsync(userId) : null;
            return user != null ? await GenerateJwtAsync(user) : null;
        }

        internal async Task<string> GenerateJwtAsync(ApplicationUser user, string[] specificRoles = null)
        {
            var token = GenerateEncryptedToken(GetSigningCredentials(), await GetClaimsAsync(user, specificRoles));
            return token;
        }

        public async Task<IEnumerable<Claim>> GetClaimsAsync(ApplicationUser user, string[] specificRoles = null)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();
            var permissionClaims = new List<Claim>();
            foreach (var role in roles)
            {
                var thisRole = await _roleManager.FindByNameAsync(role);
                if (specificRoles == null || specificRoles.Length == 0 || specificRoles.Contains(thisRole.Id) || specificRoles.Contains(thisRole.Name))
                {
                    roleClaims.Add(new Claim(ClaimTypes.Role, role));
                    var allPermissionsForThisRoles = await _roleManager.GetClaimsAsync(thisRole);
                    permissionClaims.AddRange(allPermissionsForThisRoles);
                }
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Name, user.FirstName ?? string.Empty),
                new(ClaimTypes.Surname, user.LastName ?? string.Empty),
                new(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty),
                new(ClaimTypes.StreetAddress, user.UserInfo?.Addresses?.FirstOrDefault()?.Street ?? string.Empty),
                new(ClaimTypes.PostalCode, user.UserInfo?.Addresses?.FirstOrDefault()?.PostalCode ?? string.Empty),
                new(ClaimTypes.Country, user.UserInfo?.Addresses?.FirstOrDefault()?.Country ?? string.Empty)
            }
            .Union(userClaims)
            .Union(roleClaims)
            .Union(permissionClaims)
            .ToList();

            return claims;
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private string GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
        {
            var token = new JwtSecurityToken(
               claims: claims,
               expires: DateTime.UtcNow.AddDays(ApplicationConstants.Session.SecurityTokenExpiryInDays),
               signingCredentials: signingCredentials);
            var tokenHandler = new JwtSecurityTokenHandler();
            var encryptedToken = tokenHandler.WriteToken(token);
            return encryptedToken;
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appConfig.AppConfiguration.Secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                RoleClaimType = ClaimTypes.Role,
                ClockSkew = TimeSpan.Zero
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException(_localizer["Invalid token"]);
            }

            return principal;
        }

        private SigningCredentials GetSigningCredentials()
        {
            var secret = Encoding.UTF8.GetBytes(_appConfig.AppConfiguration.Secret);
            return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
        }

        public async Task SetUserOnlineStatusAsync(string userId, bool isOnline)
        {
            var user = await _userManager.FindByIdFullyLoadedAsync(userId);
            if (user != null && (user.UserInfo == null || user.UserInfo.IsOnline != isOnline))
            {
                user.UserInfo ??= new UserInformations();
                user.UserInfo.IsOnline = isOnline;
                await _userManager.UpdateAsync(user);
                _ = _serviceProvider.GetService<IMediator>().PublishClientEvent(new UserOnlineStatusChanged(user.MapTo<UserResponse>()));
            }
        }
    }
}