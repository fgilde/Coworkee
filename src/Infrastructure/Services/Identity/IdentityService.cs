using CleanArchitectureBase.Application.Configurations;
using CleanArchitectureBase.Infrastructure.Models.Identity;
using CleanArchitectureBase.Application.Requests.Identity;
using CleanArchitectureBase.Shared.Wrapper;
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
using CleanArchitectureBase.Application.Common.Models.Identity;
using CleanArchitectureBase.Application.Contracts.Attributes;
using CleanArchitectureBase.Application.Contracts.Services.Identity;
using CleanArchitectureBase.Shared.Constants.Application;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Nextended.Core.Extensions;
using CleanArchitectureBase.Application.Common.Extensions;
using CleanArchitectureBase.Application.Hubs.Events;
using CleanArchitectureBase.Domain.Entities.Identity;

namespace CleanArchitectureBase.Infrastructure.Services.Identity
{
    [RegisterAs(typeof(ITokenService), 1, RegisterAsImplementation = true, ServiceLifetime = ServiceLifetime.Transient)]
    public class IdentityService : ITokenService
    {
        private const string InvalidErrorMessage = "Invalid email or password.";

        private readonly IServiceProvider _serviceProvider;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
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
            _appConfig = appConfig.Value;
            _localizer = localizer;
            _contextAccessor = contextAccessor;
        }

        public async Task<Result<TokenResponse>> LoginAsync(TokenRequest model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
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

            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(ApplicationConstants.Session.RefreshTokenExpiryInDays);
            user.UserInfo ??= new UserInformations();
            user.UserInfo.LastLoginDate = DateTime.UtcNow;
            user.UserInfo.IsOnline = true;
            await _userManager.UpdateAsync(user);
            _ = _serviceProvider.GetService<IMediator>().PublishClientEvent(new UserOnlineStatusChanged(user.MapTo<UserResponse>()));

            var token = await GenerateJwtAsync(user);
            var response = new TokenResponse { Token = token, RefreshToken = user.RefreshToken, UserImageURL = user.ProfilePictureDataUrl };
            _contextAccessor.HttpContext?.Session?.SetString(ApplicationConstants.Session.SessionUserIdKey, user.Id);

            //_= SetUserOnlineStatusAsync(user, true)
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
            var token = GenerateEncryptedToken(GetSigningCredentials(), await GetClaimsAsync(user));
            user.RefreshToken = GenerateRefreshToken();
            await _userManager.UpdateAsync(user);

            var response = new TokenResponse { Token = token, RefreshToken = user.RefreshToken, RefreshTokenExpiryTime = user.RefreshTokenExpiryTime };
            return await Result<TokenResponse>.SuccessAsync(response);
        }

        internal async Task<string> GenerateJwtAsync(ApplicationUser user)
        {
            var token = GenerateEncryptedToken(GetSigningCredentials(), await GetClaimsAsync(user));
            return token;
        }

        public async Task<IEnumerable<Claim>> GetClaimsAsync(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();
            var permissionClaims = new List<Claim>();
            foreach (var role in roles)
            {
                roleClaims.Add(new Claim(ClaimTypes.Role, role));
                var thisRole = await _roleManager.FindByNameAsync(role);
                var allPermissionsForThisRoles = await _roleManager.GetClaimsAsync(thisRole);
                permissionClaims.AddRange(allPermissionsForThisRoles);
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Name, user.FirstName),
                new(ClaimTypes.Surname, user.LastName),
                new(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty),
                new(ClaimTypes.StreetAddress, user.UserInfo?.Addresses?.FirstOrDefault()?.Street ?? string.Empty),
                new(ClaimTypes.PostalCode, user.UserInfo?.Addresses?.FirstOrDefault()?.PostalCode ?? string.Empty),
                new(ClaimTypes.Country, user.UserInfo?.Addresses?.FirstOrDefault()?.Country ?? string.Empty)
            }
            .Union(userClaims)
            .Union(roleClaims)
            .Union(permissionClaims);

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
            var user = await _userManager.FindByIdAsync(userId);
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