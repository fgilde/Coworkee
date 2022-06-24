using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
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
            _memoryCache = memoryCache;
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

        [HttpGet("~/Logo.svg")]
        public IActionResult LogoSvg()
        {
            return File(Encoding.UTF8.GetBytes(CustomIcons.ApplicationMainIcon), "image/svg+xml");
        }

        [HttpGet("~/Logo.png")]
        public IActionResult Logo(int? height, int? width)
        {
            return Check.TryCatch<IActionResult, Exception>(() =>
            {
                var svgDoc = SvgDocument.FromSvg<SvgDocument>(CustomIcons.ApplicationMainIcon);
                if (height.HasValue)
                    svgDoc.Height = height.Value;
                if (width.HasValue)
                    svgDoc.Width = width.Value;
                var image = svgDoc.Draw();
                return File(image.ToByteArray(), image.GetMimeType());
            });
        }

        [HttpGet("~/package/download/nuget")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> DownloadNugetPackage()
        {
            var file = Directory.EnumerateFiles("wwwroot/sdk", "*.nupkg").FirstOrDefault();
            if (!string.IsNullOrEmpty(file) && System.IO.File.Exists(file))
                return File(await System.IO.File.ReadAllBytesAsync(file), "application/zip", Path.GetFileName(file));
            return NotFound();
        }

        [HttpGet("~/{objectName}/resources.js")]
        public async Task<IActionResult> GetResources(string objectName, string cacheBuster = "")
        {
            return Ok(await _memoryCache.GetOrCreateAsync(
                CultureInfo.CurrentUICulture.TwoLetterISOLanguageName + objectName + "_resources.js" + cacheBuster,
                _ => new JsStringBuilder(false, objectName)
                        .Append(typeof(ApplicationConstants))
                        .Append(typeof(CustomIcons))
                        .ToJsonAsync()));
        }
    }

}