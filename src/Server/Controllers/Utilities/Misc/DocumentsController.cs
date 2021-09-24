using System.Threading;
using CleanArchitectureBase.Application.Features.Documents.Commands.AddEdit;
using CleanArchitectureBase.Application.Features.Documents.Commands.Delete;
using CleanArchitectureBase.Application.Features.Documents.Queries.GetAll;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
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
        [Produces(typeof(PaginatedResult<GetAllDocumentsResponse>))]
        public async Task<IActionResult> GetAll([FromQuery] GetAllDocumentsQuery query, CancellationToken cancellationToken = default)
        {
            return Ok(await _mediator.Send(query, cancellationToken));
        }

        /// <summary>
        /// Get Document By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 Ok</returns>
        [Authorize(Policy = Permissions.Documents.View)]
        [HttpGet("{id}")]
        [Produces(typeof(Result<GetDocumentByIdResponse>))]

        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
        {
            return Ok(await _mediator.Send(new GetDocumentByIdQuery { Id = id }, cancellationToken));
        }

        /// <summary>
        /// Add/Edit Document
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Documents.Create)]
        [HttpPost]
        [Produces(typeof(Result<int>))]

        public async Task<IActionResult> Post(AddEditDocumentCommand command, CancellationToken cancellationToken = default)
        {
            return Ok(await _mediator.Send(command, cancellationToken));
        }

        /// <summary>
        /// Delete a Document
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Documents.Delete)]
        [HttpDelete("{id}")]
        [Produces(typeof(Result<int>))]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            return Ok(await _mediator.Send(new DeleteDocumentCommand { Id = id }, cancellationToken));
        }
    }
}