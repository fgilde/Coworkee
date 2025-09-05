using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using lib.Coworkee.Application.Common.Models;
using lib.Coworkee.Application.Common.Security;
using lib.Coworkee.Application.Features.Translations.Commands.AddEdit;
using lib.Coworkee.Application.Features.Translations.Queries.GetAll;
using Coworkee.Server.Middlewares;
using Coworkee.Shared;
using lib.Coworkee.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Coworkee.Server.Controllers.Translations
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