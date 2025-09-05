using System.Collections.Generic;
using System.Linq;
using lib.Coworkee.Application.Contracts.Hubs;
using lib.Coworkee.Application.Hubs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nextended.Core.Extensions;
using lib.Coworkee.Application.Common.Extensions;
using lib.Coworkee.Application.Configurations;
using Coworkee.Shared;

namespace Coworkee.Server.Controllers
{
    /// <summary>
    /// Abstract BaseApi Controller Class
    /// </summary>
    [ApiController]
    //[EnableQuery]
    [ApiVersion(ApiVersions.V1)]
    [Route("api/v{version:apiVersion}/[controller]")]
    public abstract class BaseApiController<T> : ControllerBase
    {
        private IMediator _mediatorInstance;
        private ILogger<T> _loggerInstance;
        protected IMediator Mediator => _mediatorInstance ??= Get<IMediator>();
        protected ServerConfiguration Configuration => Get<ServerConfiguration>();
        protected ILogger<T> Logger => _loggerInstance ??= Get<ILogger<T>>();
        protected TService Get<TService>() => HttpContext.RequestServices.GetService<TService>();
        protected IEnumerable<TService> GetAll<TService>() => HttpContext.RequestServices.GetServices<TService>();
        protected string ControllerName => ControllerContext.ActionDescriptor.ControllerName;
        protected IHubContext<ClientEventHub, IClientEventHub> ClientEventHub => Get<IHubContext<ClientEventHub, IClientEventHub>>();
        protected int UnhashId(string hash) => hash.MapTo<int>();
        protected int[] UnhashIds(string hash) => hash.MapTo<int[]>();
        protected int[] UnhashIds(string[] hashes) => hashes.MapElementsTo<int>().ToArray();

        protected IList<TItem> Filter<TItem>(IList<TItem> l, TransferableExpression<TItem> expression)
        {
            if (expression == null || string.IsNullOrWhiteSpace(expression) || expression == "{}")
                return l;
            var fn = ODataQueryOptionsExtensions.ParseExpression<TItem>(expression);
            return fn == null ? l : l.Where(fn.Compile()).ToList();
        }

        protected IQueryable<TItem> Filter<TItem>(IQueryable<TItem> l, TransferableExpression<TItem> expression)
        {
            if (expression == null || string.IsNullOrWhiteSpace(expression) || expression == "{}")
                return l;
            var fn = ODataQueryOptionsExtensions.ParseExpression<TItem>(expression);
            return fn == null ? l : l.Where(fn);
        }
    }

}