using System;
using System.Net.Http;
using Coworkee.Application.Contracts.Services.Identity;
using Coworkee.Application.Requests.Identity;
using Coworkee.Shared.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Nextended.Core.Extensions;

namespace Coworkee.Application.IntegrationTests.Base;

public class TestFixture : IDisposable
{
    public const string BaseUrl = "https://localhost:54321";

    public ServerApplicationFactory Factory { get; private set; }
    public HttpClient UnauthorizedClient { get; private set; }


    public static string ApiUrl(string path = "") => $"{BaseUrl}/api/v1/{path}";

    private WebApplicationFactoryClientOptions _clientOptions = new WebApplicationFactoryClientOptions()
    {
        AllowAutoRedirect = true,
        HandleCookies = true,
        BaseAddress = new Uri(BaseUrl)
    };
    public TestFixture()
    {
        Factory = new ServerApplicationFactory();        
        UnauthorizedClient = Factory.CreateClient(_clientOptions);
    }

    public HttpClient CreateAuthorizedClient(CreateUser user) 
        => SetAuthToken(Factory.CreateClient(_clientOptions), user);

    private HttpClient SetAuthToken(HttpClient res, CreateUser user)
    {
        var tokenService = Factory.Services.GetService<ITokenService>();
        var model = user.MapTo<TokenRequest>();
        var tokenResonse = tokenService.LoginAsync(model).Result;
        res.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, tokenResonse.Data.Token);
        //var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        //var token = tokenHandler.ReadJwtToken(tokenResonse.Data.Token);
        //var claims = token.Claims.ToList();
        return res;
    }

    public void Dispose()
    {
    }
}
