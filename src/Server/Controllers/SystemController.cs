using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Features.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using CleanArchitectureBase.Application.Configurations;
using CleanArchitectureBase.Server.Extensions;
using CleanArchitectureBase.Shared.Constants.Application;
using CleanArchitectureBase.Shared.Constants.Role;
using Newtonsoft.Json;
using Nextended.Core.Extensions;


namespace CleanArchitectureBase.Server.Controllers
{
    public class SystemController : BaseApiController<SystemController>
    {
        [Authorize]
        [HttpGet(nameof(AuthorizeServerUrl))]
        public ActionResult<string> AuthorizeServerUrl(string url)
        {
            var token = Request.Headers.Authorization.ToString().Split(" ")[1]; // remove Scheme
            return Ok(UriExtensions.AddParameterToUrl(url, ApplicationConstants.ParameterNames.AuthedUrlParameter, token));
        }

        [Authorize]
        [HttpGet(nameof(UnhashHashedId))]
        [Produces(typeof(int))]
        // TODO: Remove after support of hashed ids is added to extended attributes
        public ActionResult<string> UnhashHashedId(string id)
        {
            return Ok(UnhashId(id));
        }

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


        [Authorize(Roles = RoleConstants.AdministratorRole)]
        [HttpGet(nameof(SystemConfiguration))]
        [Produces(typeof(ServerConfiguration))]
        public IActionResult SystemConfiguration()
        {
            return Ok(Configuration);
        }

        [Authorize(Roles = RoleConstants.AdministratorRole)]
        [HttpPost(nameof(WriteSystemConfiguration))]
        public IActionResult WriteSystemConfiguration([FromBody] ServerConfiguration config)
        {
            // For temporary set
            foreach (var item in config.ToFlatDictionary())
                Get<IConfiguration>()[item.Key.Replace(".", ":")] = item.Value;
            // For persistent
            //System.IO.File.WriteAllText(Path.Combine(AppContext.BaseDirectory, "appSettings.json"), JsonConvert.SerializeObject(config, Formatting.Indented));
            //Get<IHostApplicationLifetime>().StopApplication();
            return Ok();
        }


        //[HttpPost(nameof(SendNotification))]
        //public async Task<IActionResult> SendNotification()
        //{
        //    await Get<INotificationService>().SendAsync(new NotificationRequest()
        //    {
        //        Target = EventTarget.All,
        //        Content = "Test",
        //        Excerpt = "Tes",
        //        PersistInDb = false,
        //        SendAsMail = NotificationAsMail.Never,
        //        Subject = "Moin moin"
        //    });
        //    return Ok();
        //}

    }
}