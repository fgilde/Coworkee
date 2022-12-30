using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Features.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using CleanArchitectureBase.Application.Configurations;
using CleanArchitectureBase.Server.Extensions;
using CleanArchitectureBase.Shared.Constants.Application;
using Nextended.Core.Extensions;
using CleanArchitectureBase.Shared.Constants.Role;
using System.IO;
using Microsoft.Extensions.Hosting;

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
        // TODO: Remove after support of hashed ids has been added to extended attributes
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
            var configuration = Get<IConfiguration>();
            foreach (var item in config.ToFlatDictionary())
                configuration[item.Key.Replace(".", ":")] = item.Value;
            // For persistent
            var path = Path.Combine(Directory.GetCurrentDirectory(), ApplicationConstants.FileAccess.OverridingSettingsFile);
            System.IO.File.WriteAllText(path, JsonConvert.SerializeObject(config, Formatting.Indented));
            return Ok();
        }

        [Authorize(Roles = RoleConstants.AdministratorRole)]
        [HttpPost(nameof(RestoreSystemConfiguration))]
        [Produces(typeof(ServerConfiguration))]
        public IActionResult RestoreSystemConfiguration()
        {
            var configuration = Get<IConfiguration>();
            var path = Path.Combine(Directory.GetCurrentDirectory(), ApplicationConstants.FileAccess.OverridingSettingsFile);
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);
            var config = configuration as ConfigurationRoot;
            config?.Reload();
            return Ok(configuration.BindTo<ServerConfiguration>());
        }


        [Authorize(Roles = RoleConstants.AdministratorRole)]
        [HttpPost(nameof(RestartServer))]
        public IActionResult RestartServer()
        {
            Get<IHostApplicationLifetime>().StopApplication();
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