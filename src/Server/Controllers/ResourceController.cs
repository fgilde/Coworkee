using System.Globalization;
using System.Threading.Tasks;
using CleanArchitectureBase.Shared.Constants;
using CleanArchitectureBase.Shared.Constants.Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Nextended.Core.Helper;

namespace CleanArchitectureBase.Server.Controllers
{
    public class ResourceController : BaseApiController<ResourceController>
    {
        private readonly IMemoryCache memoryCache;

        public ResourceController(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }

        [HttpGet("~/{objectName}/resources.js")]
        public async Task<IActionResult> GetResources(string objectName, string cacheBuster = "")
        {
            string jsonResources = await memoryCache.GetOrCreateAsync(
                CultureInfo.CurrentUICulture.TwoLetterISOLanguageName + objectName +"_resources.js" + cacheBuster,
                cacheEntry =>
                    new JsStringBuilder(false, objectName)
                        .Append(typeof(ApplicationConstants))
                        .Append(typeof(CustomIcons))
                        .ToJsonAsync());

            return Ok(jsonResources);
        }
    }
}