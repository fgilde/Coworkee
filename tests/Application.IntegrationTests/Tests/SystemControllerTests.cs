using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Coworkee.Application.Common.Models;
using Coworkee.Application.IntegrationTests.Base;
using Coworkee.Shared.Constants.Application;
using SDK;
using Xunit;

namespace Coworkee.Application.IntegrationTests.Tests
{
    public class SystemControllerTests(TestFixture fixture) : BaseTest(fixture)
    {
        [Fact]
        public async Task CanResolveVersion()
        {            
            var versionRes1 = await AdminApiClient.System_VersionAsync();
            var versionRes2 = await UnauthorizedApiClient.System_VersionAsync();
            var responseFromAsync = await AdminUserHttpClient.GetAsync("api/v1/System/Version");                        
            var versionRes3 = await responseFromAsync.Content.ReadFromJsonAsync<VersionInfoModel>();
            
            Assert.NotNull(versionRes1);
            Assert.NotNull(versionRes2);
            Assert.NotNull(versionRes3);
            Assert.Equal(versionRes1.System, versionRes2.System);
            Assert.Equal(versionRes1.System, versionRes3.System);
            Assert.NotEmpty(versionRes1.System);
        }

        [Fact]
        public async Task CanAuthorizeAnUrl()
        {
            var result = await AdminApiClient.System_AuthorizeServerUrlAsync("http://localhost:5000/sample");
            Assert.NotNull(result);            
            Assert.True(result.Contains(ApplicationConstants.ParameterNames.AuthedUrlParameter));
        }

        [Fact]
        public async Task HandlesNotAuthorized()
        {
            var allowedResponse = await AdminUserHttpClient.GetAsync("api/v1/System/AuthorizeServerUrl?url=t");
            var notAllowedResponse = await UnauthorizedHttpClient.GetAsync("api/v1/System/AuthorizeServerUrl?url=t");
            Assert.Equal(HttpStatusCode.OK, allowedResponse.StatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, notAllowedResponse.StatusCode);
            bool correctExceptionThrown = false;
            try
            {
                var result = await UnauthorizedApiClient.System_AuthorizeServerUrlAsync("http://localhost:5000/sample");
            }
            catch (ApiException e)
            {                
                correctExceptionThrown = e.StatusCode == 401;
            }
            Assert.True(correctExceptionThrown);
        }

        [Fact]
        public async Task HandlesForbidden()
        {
            var allowedResponse = await AdminUserHttpClient.GetAsync("api/v1/System/SystemConfiguration");
            var notAllowedResponse = await UnauthorizedHttpClient.GetAsync("api/v1/System/SystemConfiguration");
            var forbiddenResponse = await BasicUserHttpClient.GetAsync("api/v1/System/SystemConfiguration");
            Assert.Equal(HttpStatusCode.OK, allowedResponse.StatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, notAllowedResponse.StatusCode);
            Assert.Equal(HttpStatusCode.Forbidden, forbiddenResponse.StatusCode);
            bool correctExceptionThrown = false;
            try
            {
                var result = await BasicApiClient.System_SystemConfigurationAsync();
            }
            catch (ApiException e)
            {
                correctExceptionThrown = e.StatusCode == 403;
            }
            Assert.True(correctExceptionThrown);
        }

    }
}
