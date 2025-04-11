using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Coworkee.Application.Common.Models;
using Coworkee.Application.Features.Brands.Commands.AddEdit;
using Coworkee.Application.Features.Brands.Commands.Delete;
using Coworkee.Application.Features.Brands.Queries.Export;
using Coworkee.Application.Features.Brands.Queries.GetAll;
using Coworkee.Application.Features.Brands.Queries.GetById;
using Coworkee.Server.Filters;
using Coworkee.Server.Middlewares;
using Coworkee.Shared;
using Coworkee.Shared.Constants.Permission;
using HeyRed.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nextended.Core;


namespace Coworkee.Server.Controllers.Catalog
{
    public class BrandsController : BaseApiController<BrandsController>
    {

        /// <summary>
        /// Get All Brands
        /// </summary>
        /// <returns>Status 200 OK</returns>
        //[ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Policy = Permissions.Brands.View)]
        [HttpGet]
        [Produces(typeof(IReadOnlyCollection<BrandDto>))]        
        public async Task<IActionResult> GetAll([FromOdataFilter] TransferableExpression<BrandDto> filter = null, CancellationToken cancellationToken = default)
        {
            var brands = await Mediator.Send(new GetAllBrandsQuery { OdataFilterQuery = filter}, cancellationToken);
            return Ok(brands);
        }

        /// <summary>
        /// Get a Brand By Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Status 200 Ok</returns>
        [Authorize(Policy = Permissions.Brands.View)]
        [HttpGet("{id}")]
        [Produces(typeof(BrandDto))]
        public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken = default)
        {
            var brand = await Mediator.Send(new GetBrandByIdQuery(UnhashId(id)), cancellationToken);
            return Ok(brand);
        }

        /// <summary>
        /// Create/Update a Brand
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Brands.Create)]
        [HttpPost]
        public async Task<IActionResult> Post(AddEditBrandsCommand command, CancellationToken cancellationToken = default)
        {
            return Ok(await Mediator.Send(command, cancellationToken));
        }

        /// <summary>
        /// Delete a Brand
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Brands.Delete)]
        [HttpDelete]
        public async Task<IActionResult> Delete(string[] ids, CancellationToken cancellationToken = default)
        {
            await Mediator.Send(new DeleteBrandCommand {Ids = UnhashIds(ids)}, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Exports brands
        /// </summary>
        /// <returns></returns>
        [Authorize(Policy = Permissions.Brands.Export)]
        [HttpGet(nameof(Export))]
        [AllowSynchronousIo]
        public async Task<IActionResult> Export([FromQuery] ExportBrandsQuery query, CancellationToken cancellationToken = default)
        {
            var res = await Mediator.Send(query, cancellationToken);
            var mimeType = MimeGuesser.GuessMimeType(res);
            var fileDownloadName = $"{ControllerContext.ActionDescriptor.ControllerName}-{DateTime.Now:ddMMyyyyHHmmss}.{MimeType.GetExtension(mimeType)}";
            return File(res, mimeType, fileDownloadName);
        }
    }
}