using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AKSoftware.Localization.MultiLanguages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Nextended.Core;

namespace CleanArchitectureBase.Server.Middlewares
{
    public class RequestCultureMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestCultureMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var languageContainerService = context.RequestServices.GetService<ILanguageContainerService>();
            var cultureQuery = context.Request.Query["culture"];
            CultureInfo culture = null;
            if (!string.IsNullOrWhiteSpace(cultureQuery))
            {
                culture = new CultureInfo(cultureQuery);

                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;
            }
            else if (context.Request.Headers.ContainsKey("Accept-Language"))
            {
                var cultureHeader = context.Request.Headers["Accept-Language"];
                if (cultureHeader.Any())
                {
                    culture = new CultureInfo(cultureHeader.First().Split(',').First().Trim());

                    CultureInfo.CurrentCulture = culture;
                    CultureInfo.CurrentUICulture = culture;
                }
            }
            if (languageContainerService != null && culture != null)
                Check.TryCatch<Exception>(() => languageContainerService.SetLanguage(culture));

            await _next(context);
        }
    }
}