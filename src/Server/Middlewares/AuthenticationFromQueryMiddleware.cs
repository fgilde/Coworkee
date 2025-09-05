using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using lib.Coworkee.Application.Configurations;
using Coworkee.Shared;
using lib.Coworkee.Shared.Constants.Application;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Primitives;

namespace Coworkee.Server.Middlewares
{

    public class AuthenticationFromQueryMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ServerConfiguration _configuration;

        public AuthenticationFromQueryMiddleware(RequestDelegate next, ServerConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Query.TryGetValue(ApplicationConstants.ParameterNames.AuthedUrlParameter, out StringValues query))
            {
                if (!context.Request.Headers.Authorization.Any())
                {
                    var header = $"{JwtBearerDefaults.AuthenticationScheme} {query}";
                    var items = context.Request.Query.SelectMany(x => x.Value, (col, value) => new KeyValuePair<string, string>(col.Key, value)).ToList();
                    items.RemoveAll(x => x.Key == ApplicationConstants.ParameterNames.AuthedUrlParameter); // Remove all values for key
                    var qb = new QueryBuilder(items);
                    context.Request.Headers.Authorization = header;
                    context.Request.QueryString = qb.ToQueryString();
                    var claim = ClaimReader.ReadClaimsFromJwt(query);
                    var userId = claim.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
                    context.Session.SetString(ApplicationConstants.Session.SessionUserIdKey, userId);
                    context.Response.Redirect(context.Request.Path);
                    return;
                }
            }
            await _next.Invoke(context);
        }

    }

    public static class AuthenticationFromQueryExtensions
    {
        public static IApplicationBuilder UseAuthenticationFromQuery(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthenticationFromQueryMiddleware>();
        }
    }
}