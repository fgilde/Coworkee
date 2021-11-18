using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Features.DocumentTypes.Commands.AddEdit;
using CleanArchitectureBase.Application.Features.DocumentTypes.Commands.Delete;
using CleanArchitectureBase.Application.Features.DocumentTypes.Queries.Export;
using CleanArchitectureBase.Application.Features.DocumentTypes.Queries.GetAll;
using CleanArchitectureBase.Application.Features.DocumentTypes.Queries.GetById;
using CleanArchitectureBase.Server.Filters;
using CleanArchitectureBase.Shared.Constants.Permission;
using HeyRed.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitectureBase.Server.Controllers.Utilities.Misc
{
    [ApiController]
    public class DocumentTypesController : BaseApiController<DocumentTypesController>
    {
        /// <summary>
        /// Get All Document Types
        /// </summary>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.DocumentTypes.View)]
        [HttpGet]
        [Produces(typeof(ReadOnlyCollection<DocumentTypeDto>))]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
        {
            return Ok(await Mediator.Send(new GetAllDocumentTypesQuery(), cancellationToken));
        }

        /// <summary>
        /// Get Document Type By Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Status 200 Ok</returns>
        [Authorize(Policy = Permissions.DocumentTypes.View)]
        [HttpGet("{id}")]
        [Produces(typeof(DocumentTypeDto))]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
        {
            return Ok(await Mediator.Send(new GetDocumentTypeByIdQuery(id), cancellationToken));
        }

        /// <summary>
        /// Create/Update a Document Type
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.DocumentTypes.Create)]
        [HttpPost]
        public async Task<IActionResult> Post(AddEditDocumentTypesCommand command, CancellationToken cancellationToken = default)
        {
            return Ok(await Mediator.Send(command, cancellationToken));
        }

        /// <summary>
        /// Delete a Document Type
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.DocumentTypes.Delete)]
        [HttpDelete]
        public async Task<IActionResult> Delete(int[] ids, CancellationToken cancellationToken = default)
        {
            return Ok(await Mediator.Send(new DeleteDocumentTypesCommand { Ids = ids }, cancellationToken));
        }

        /// <summary>
        /// Exports Document Types
        /// </summary>
        /// <returns></returns>
        [Authorize(Policy = Permissions.DocumentTypes.Export)]
        [HttpGet(nameof(Export))]
        [AllowSynchronousIo]
        public async Task<IActionResult> Export([FromQuery] ExportDocumentTypesQuery query, CancellationToken cancellationToken = default)
        {
            var res = await Mediator.Send(query, cancellationToken);
            var mimeType = MimeGuesser.GuessMimeType(res);
            var fileDownloadName = $"{ControllerContext.ActionDescriptor.ControllerName}-{DateTime.Now:ddMMyyyyHHmmss}.{MimeTypesMap.GetExtension(mimeType)}";
            return File(res, mimeType, fileDownloadName);
        }
    }
}