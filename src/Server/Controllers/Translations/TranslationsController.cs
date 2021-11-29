using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Common.Security;
using CleanArchitectureBase.Application.Features.Translations.Commands.AddEdit;
using CleanArchitectureBase.Application.Features.Translations.Commands.Delete;
using CleanArchitectureBase.Application.Features.Translations.Export;
using CleanArchitectureBase.Application.Features.Translations.Import;
using CleanArchitectureBase.Application.Features.Translations.Queries.GetAll;
using CleanArchitectureBase.Application.Features.Translations.Queries.GetAllPaged;
using CleanArchitectureBase.Application.Features.Translations.Queries.GetById;
using CleanArchitectureBase.Server.Extensions;
using CleanArchitectureBase.Server.Filters;
using CleanArchitectureBase.Shared.Constants.Permission;
using CleanArchitectureBase.Shared.Wrapper;
using HeyRed.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        [Filters.CustomAuthorize(Policies = new[] { Permissions.Translations.View })]
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
        [Filters.CustomAuthorize(Policies = new[] { Permissions.Translations.View })]
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
        [Filters.CustomAuthorize(Policies = new[] { Permissions.Translations.Create, Permissions.Translations.Edit }, PolicyMatch = PolicyMatch.Any)]
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
        [Authorize(Policy = Permissions.Translations.Delete)]
        [HttpDelete]
        public async Task<IActionResult> Delete(int[] ids, CancellationToken cancellationToken = default)
        {
            return Ok(await Mediator.Send(new DeleteTranslationsCommand { Ids = ids }, cancellationToken));
        }

        /// <summary>
        /// Exports products
        /// </summary>
        /// <returns></returns>
        [Authorize(Policy = Permissions.Translations.Export)]
        [HttpPost(nameof(Export))]
        [AllowSynchronousIo]
        public async Task<IActionResult> Export(ExportTranslationsQuery query, CancellationToken cancellationToken = default)
        {
            var res = await Mediator.Send(query, cancellationToken);
            var mimeType = MimeGuesser.GuessMimeType(res);
            var culturePart = query.FilterByCurrentCulture ? $"{CultureInfo.CurrentUICulture}-" : "";
            var fileDownloadName = $"{ControllerContext.ActionDescriptor.ControllerName}-{culturePart}{DateTime.Now:ddMMyyyyHHmmss}.{MimeTypesMap.GetExtension(mimeType)}";
            return File(res, mimeType, fileDownloadName);
        }

        /// <summary>
        /// Update Profile Picture
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Status 200 OK</returns>
        [HttpPost(nameof(ImportFile))]

        public async Task<IActionResult> ImportFile(IFormFile file, CancellationToken cancellationToken = default)
        {
            var bytes = await file.GetBytesAsync(cancellationToken);
            return Ok(await Mediator.Send(new ImportTranslationsQuery
            {
                ContentType = MimeGuesser.GuessMimeType(bytes),
                Data = bytes
            }, cancellationToken));
        }

        /// <summary>
        /// Update Profile Picture
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Status 200 OK</returns>
        [HttpPost(nameof(Import))]

        public async Task<IActionResult> Import(ImportTranslationsQuery request, CancellationToken cancellationToken = default)
        {
            request.ContentType = MimeGuesser.GuessMimeType(request.Data);
            return Ok(await Mediator.Send(request, cancellationToken));
        }

    }
}