using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Dtos;
using CleanArchitectureBase.Application.Features.Translations.Commands.AddEdit;
using CleanArchitectureBase.Application.Features.Translations.Commands.Delete;
using CleanArchitectureBase.Application.Features.Translations.Queries.GetAll;
using CleanArchitectureBase.Application.Features.Translations.Queries.GetAllPaged;
using CleanArchitectureBase.Application.Features.Translations.Queries.GetById;
using CleanArchitectureBase.Application.Security;
using CleanArchitectureBase.Shared.Constants.Permission;
using CleanArchitectureBase.Shared.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitectureBase.Server.Controllers.Translations
{
    public class TranslationsController : BaseApiController<TranslationsController>
    {
        /// <summary>
        /// Get All Translations
        /// </summary>
        /// <returns>Status 200 OK</returns>
        // [Authorize(Policy = Permissions.Products.View)]
        [Filters.CustomAuthorize(Policies = new[] { Permissions.Products.View })]
        [HttpGet(nameof(GetAllPaged))]
        [Produces(typeof(PaginatedResult<TranslationDto>))]
        public async Task<IActionResult> GetAllPaged([FromQuery] GetAllTranslationsPagedQuery query, CancellationToken cancellationToken = default)
        {
            return Ok(await Mediator.Send(query, cancellationToken));
        }

        /// <summary>
        /// Get All Translations
        /// </summary>
        /// <returns>Status 200 OK</returns>
        // [Authorize(Policy = Permissions.Products.View)]
        [AllowAnonymous]
        [HttpGet(nameof(GetAll))]
        [Produces(typeof(ReadOnlyCollection<TranslationDto>))]
        public async Task<IActionResult> GetAll([FromQuery] GetAllTranslationsQuery query, CancellationToken cancellationToken = default)
        {
            return Ok(await Mediator.Send(query, cancellationToken));
        }


        /// <summary>
        /// Get a translation by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Status 200 Ok</returns>
        [Filters.CustomAuthorize(Policies = new[] { Permissions.Products.View })]
        [HttpGet("{id}")]
        [Produces(typeof(Result<TranslationDto>))]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
        {
            var product = await Mediator.Send(new GetTranslationByIdQuery(id), cancellationToken);
            return Ok(product);
        }

        /// <summary>
        /// Add/Edit one or more Translations
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Status 200 OK</returns>
        [Filters.CustomAuthorize(Policies = new[] { Permissions.Products.Create, Permissions.Products.Edit }, PolicyMatch = PolicyMatch.Any)]
        [HttpPost]
        public async Task<IActionResult> Post(AddEditTranslationsCommand command, CancellationToken cancellationToken = default)
        {
            return Ok(await Mediator.Send(command, cancellationToken));
        }

        /// <summary>
        /// Delete a Translations with given ids
        /// </summary>
        /// <param name="ids">Products to delete</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Status 200 OK response</returns>
        [Authorize(Policy = Permissions.Products.Delete)]
        [HttpDelete]
        [Produces(typeof(Result))]
        public async Task<IActionResult> Delete(int[] ids, CancellationToken cancellationToken = default)
        {
            return Ok(await Mediator.Send(new DeleteTranslationsCommand { Ids = ids }, cancellationToken));
        }

    }
}