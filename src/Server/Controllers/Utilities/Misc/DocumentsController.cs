using System.Threading;
using CleanArchitectureBase.Application.Features.Documents.Commands.AddEdit;
using CleanArchitectureBase.Application.Features.Documents.Commands.Delete;
using CleanArchitectureBase.Application.Features.Documents.Queries.GetAll;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Features.Documents.Queries.GetById;
using CleanArchitectureBase.Shared.Constants.Permission;
using CleanArchitectureBase.Shared.Wrapper;
using Microsoft.AspNetCore.Authorization;

namespace CleanArchitectureBase.Server.Controllers.Utilities.Misc
{
    [ApiController]
    public class DocumentsController : BaseApiController<DocumentsController>
    {
        /// <summary>
        /// Get All Documents
        /// </summary>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Documents.View)]
        [HttpGet]
        [Produces(typeof(PaginatedResult<DocumentDto>))]
        public async Task<IActionResult> GetAll([FromQuery] GetAllDocumentsQuery query, CancellationToken cancellationToken = default)
        {
            return Ok(await Mediator.Send(query, cancellationToken));
        }

        /// <summary>
        /// Get Document By Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Status 200 Ok</returns>
        [Authorize(Policy = Permissions.Documents.View)]
        [HttpGet("{id}")]
        [Produces(typeof(DocumentDto))]

        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
        {
            return Ok(await Mediator.Send(new GetDocumentByIdQuery(id), cancellationToken));
        }

        /// <summary>
        /// Add/Edit Document
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Documents.Create)]
        [HttpPost]

        public async Task<IActionResult> Post(AddEditDocumentsCommand command, CancellationToken cancellationToken = default)
        {
            return Ok(await Mediator.Send(command, cancellationToken));
        }

        /// <summary>
        /// Delete a Document
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Documents.Delete)]
        [HttpDelete]
        public async Task<IActionResult> Delete(int[] ids, CancellationToken cancellationToken = default)
        {
            return Ok(await Mediator.Send(new DeleteDocumentsCommand {Ids = ids}, cancellationToken));
        }
    }
}