using System;
using System.Globalization;
using System.Threading.Tasks;
using CleanArchitectureBase.Shared.Constants;
using CleanArchitectureBase.Shared.Constants.Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Nextended.Core;
using Nextended.Core.Helper;
using Nextended.Imaging;
using Svg;

namespace CleanArchitectureBase.Server.Controllers
{
    public class ResourceController : BaseApiController<ResourceController>
    {
        private readonly IMemoryCache _memoryCache;

        public ResourceController(IMemoryCache memoryCache)
        {
            this._memoryCache = memoryCache;
        }

        [HttpGet("~/favicon.ico")]
        public IActionResult FavIcon()
        {
            return Check.TryCatch<IActionResult, Exception>(() =>
            {
                var svgDoc = SvgDocument.FromSvg<SvgDocument>(CustomIcons.ApplicationMainIcon);
                var image = svgDoc.Draw();
                return File(image.ToByteArray(), image.GetMimeType());
            });
        }

        [HttpGet("~/{objectName}/resources.js")]
        public async Task<IActionResult> GetResources(string objectName, string cacheBuster = "")
        {
            string jsonResources = await _memoryCache.GetOrCreateAsync(
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