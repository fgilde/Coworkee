
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Features.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using CleanArchitectureBase.Application.Configurations;
using CleanArchitectureBase.Server.Extensions;

namespace CleanArchitectureBase.Server.Controllers
{
    public class SystemController : BaseApiController<SystemController>
    {
        [AllowAnonymous]
        [HttpGet(nameof(Version))]
        public async Task<ActionResult<VersionInfoModel>> Version()
        {
            return Ok(await Mediator.Send(new GetVersion.Request()));
        }

        [AllowAnonymous]
        [HttpGet(nameof(AvailableApiVersions))]
        public ActionResult<IEnumerable<Version>> AvailableApiVersions()
        {
            return Ok(ApiVersions.All.Select(v => v.ToVersion()));
        }

        [AllowAnonymous]
        [HttpGet(nameof(Configuration))]
        [Produces(typeof(Publicsettings))]
        public IActionResult GetConfiguration()
        {
            if (string.IsNullOrWhiteSpace(Configuration.ClientUrl))
                Get<IConfiguration>()[nameof(ServerConfiguration.ClientUrl)] = Request.GetRefererUris().FirstOrDefault()?.GetLeftPart(UriPartial.Scheme | UriPartial.Authority);

            return Ok(Configuration.PublicSettings);
        }

        [HttpPost("{queue}")]
        public async Task<ActionResult> SendOnServiceBus(string queue, [FromBody] ProductDto entity)
        {
            return Ok(await Mediator.Send(new SendToServiceBus.Request
            {
                Queue = queue,
                Content = entity
            }));
        }
    }
}