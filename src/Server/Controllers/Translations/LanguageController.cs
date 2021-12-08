using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Common.Security;
using CleanArchitectureBase.Application.Features.Translations.Commands.AddEdit;
using CleanArchitectureBase.Application.Features.Translations.Queries.GetAll;
using CleanArchitectureBase.Server.Middlewares;
using CleanArchitectureBase.Shared;
using CleanArchitectureBase.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitectureBase.Server.Controllers.Translations
{
    public class LanguageController : BaseApiController<LanguageController>
    {

        /// <summary>
        /// Add/Edit languages
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Status 200 OK</returns>
        [Filters.CustomAuthorize(Policies = new[] { Permissions.Translations.Create, Permissions.Translations.Edit }, PolicyMatch = PolicyMatch.Any)]
        [HttpPost]
        [Produces(typeof(AddUpdateResult<LanguageDto>))]
        public async Task<IActionResult> Post(AddEditLanguagesCommand command, CancellationToken cancellationToken = default)
        {
            return Ok(await Mediator.Send(command, cancellationToken));
        }

        /// <summary>
        /// Get All Languages
        /// </summary>
        /// <returns>Status 200 OK</returns>
        //[ApiExplorerSettings(IgnoreApi = true)]
        [AllowAnonymous]
        [HttpGet]
        [Produces(typeof(IReadOnlyCollection<LanguageDto>))]
        public async Task<IActionResult> GetAll([FromOdataFilter] TransferableExpression<LanguageDto> filter = null, CancellationToken cancellationToken = default)
        {
            var languages = await Mediator.Send(new GetAllLanguagesQuery { OdataFilterQuery = filter }, cancellationToken);
            return Ok(languages);
        }
    }
}