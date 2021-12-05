using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Features.Brands.Commands.AddEdit;
using CleanArchitectureBase.Application.Features.Brands.Commands.Delete;
using CleanArchitectureBase.Application.Features.Brands.Queries.Export;
using CleanArchitectureBase.Application.Features.Brands.Queries.GetAll;
using CleanArchitectureBase.Application.Features.Brands.Queries.GetById;
using CleanArchitectureBase.Server.Filters;
using CleanArchitectureBase.Server.Middlewares;
using CleanArchitectureBase.Shared;
using CleanArchitectureBase.Shared.Constants.Permission;
using HeyRed.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace CleanArchitectureBase.Server.Controllers.Catalog
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
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
        {
            var brand = await Mediator.Send(new GetBrandByIdQuery(id), cancellationToken);
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
        public async Task<IActionResult> Delete(int[] ids, CancellationToken cancellationToken = default)
        {
            return Ok(await Mediator.Send(new DeleteBrandCommand { Ids = ids }, cancellationToken));
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
            var fileDownloadName = $"{ControllerContext.ActionDescriptor.ControllerName}-{DateTime.Now:ddMMyyyyHHmmss}.{MimeTypesMap.GetExtension(mimeType)}";
            return File(res, mimeType, fileDownloadName);
        }
    }
}