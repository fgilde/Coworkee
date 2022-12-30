using System.Threading;
using Coworkee.Application.Features.Documents.Commands.AddEdit;
using Coworkee.Application.Features.Documents.Commands.Delete;
using Coworkee.Application.Features.Documents.Queries.GetAll;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Coworkee.Application.Common.Models;
using Coworkee.Application.Features.Documents.Queries.GetById;
using Coworkee.Shared.Constants.Permission;
using Coworkee.Shared.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Nextended.Core;

namespace Coworkee.Server.Controllers.Utilities.Misc
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
        public async Task<IActionResult> GetAll([FromQuery] GetAllDocumentsQuery query,
            CancellationToken cancellationToken = default)
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

        public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken = default)
        {
            return Ok(await Mediator.Send(new GetDocumentByIdQuery(UnhashId(id)), cancellationToken));
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
        public async Task<IActionResult> Delete(string[] ids, CancellationToken cancellationToken = default)
        {
            return Ok(await Mediator.Send(new DeleteDocumentsCommand { Ids = UnhashIds(ids) }, cancellationToken));
        }

        /// <summary>
        /// Gets the Mimetype for given url
        /// </summary>
        /// <returns>Status 200 OK</returns>
        [Authorize]
        [HttpGet(nameof(GetMimeType))]
        [Produces(typeof(string))]
        public async Task<IActionResult> GetMimeType(string url,
            CancellationToken cancellationToken = default)
        {
            var res = await MimeType.ReadMimeTypeFromUrlAsync(url, cancellationToken);
            return Ok(res);
        }
    }
}