using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Coworkee.SDK;
using Coworkee.Shared.Constants.Application;
using Coworkee.Shared.Models;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Coworkee.Application.IntegrationTests.Base;

public class BaseTest(TestFixture fixture) : IClassFixture<TestFixture>
{
    private HttpClient _adminUserHttpClient;
    private HttpClient _basicUserHttpClient;
    protected TestFixture Fixture { get; } = fixture;
    protected HttpClient UnauthorizedHttpClient => Fixture.UnauthorizedClient;
    protected HttpClient AdminUserHttpClient => _adminUserHttpClient ??= CreateClient(ApplicationConstants.Defaults.Users.Administrators[0]);
    protected HttpClient BasicUserHttpClient => _basicUserHttpClient ??= CreateClient(ApplicationConstants.Defaults.Users.Basic[0]);
    protected HttpClient CreateClient(CreateUser user) => Fixture.CreateAuthorizedClient(user);
    protected ServerApplicationFactory Factory => Fixture.Factory;
    protected IServiceProvider ServiceProvider => Factory.Services;
    protected T Get<T>() => ServiceProvider.GetRequiredService<T>();
    
    protected IApplicationClient UnauthorizedApiClient => CreateApiClient(UnauthorizedHttpClient);
    protected IApplicationClient AdminApiClient => CreateApiClient(AdminUserHttpClient);
    protected IApplicationClient BasicApiClient => CreateApiClient(BasicUserHttpClient);

    private IApplicationClient CreateApiClient(HttpClient client) => new ApplicationClient(TestFixture.ApiUrl(), client);

    [Fact]
    public async Task IsHealthy()
    {
        var response = await UnauthorizedHttpClient.GetAsync("/api/health");
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
