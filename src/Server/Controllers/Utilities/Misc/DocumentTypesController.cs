using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Features.DocumentTypes.Commands.AddEdit;
using CleanArchitectureBase.Application.Features.DocumentTypes.Commands.Delete;
using CleanArchitectureBase.Application.Features.DocumentTypes.Queries.Export;
using CleanArchitectureBase.Application.Features.DocumentTypes.Queries.GetAll;
using CleanArchitectureBase.Application.Features.DocumentTypes.Queries.GetById;
using CleanArchitectureBase.Shared.Constants.Permission;
using CleanArchitectureBase.Shared.Wrapper;
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
        [Produces(typeof(Result<List<GetAllDocumentTypesResponse>>))]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
        {
            return Ok(await _mediator.Send(new GetAllDocumentTypesQuery(), cancellationToken));
        }

        /// <summary>
        /// Get Document Type By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 Ok</returns>
        [Authorize(Policy = Permissions.DocumentTypes.View)]
        [HttpGet("{id}")]
        [Produces(typeof(Result<GetDocumentTypeByIdResponse>))]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
        {
            return Ok(await _mediator.Send(new GetDocumentTypeByIdQuery { Id = id }, cancellationToken));
        }

        /// <summary>
        /// Create/Update a Document Type
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.DocumentTypes.Create)]
        [HttpPost]
        [Produces(typeof(Result<int>))]
        public async Task<IActionResult> Post(AddEditDocumentTypeCommand command, CancellationToken cancellationToken = default)
        {
            return Ok(await _mediator.Send(command, cancellationToken));
        }

        /// <summary>
        /// Delete a Document Type
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.DocumentTypes.Delete)]
        [HttpDelete("{id}")]
        [Produces(typeof(Result<int>))]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            return Ok(await _mediator.Send(new DeleteDocumentTypeCommand { Id = id }, cancellationToken));
        }

        /// <summary>
        /// Search Document Types and Export to Excel
        /// </summary>
        /// <param name="searchString"></param>
        /// <returns></returns>
        [Authorize(Policy = Permissions.DocumentTypes.Export)]
        [HttpGet("export")]
        [Produces(typeof(Result<string>))]
        public async Task<IActionResult> Export(string searchString = "", CancellationToken cancellationToken = default)
        {
            return Ok(await _mediator.Send(new ExportDocumentTypesQuery(searchString), cancellationToken));
        }
    }
}