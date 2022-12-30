using System;
using System.Threading;
using System.Threading.Tasks;
using Coworkee.Application.Common.Models;
using Coworkee.Application.Common.Security;
using Coworkee.Application.Features.Products.Commands.AddEdit;
using Coworkee.Application.Features.Products.Commands.Delete;
using Coworkee.Application.Features.Products.Queries.Export;
using Coworkee.Application.Features.Products.Queries.GetAllPaged;
using Coworkee.Application.Features.Products.Queries.GetById;
using Coworkee.Application.Features.Products.Queries.GetProductImage;
using Coworkee.Server.Filters;
using Coworkee.Shared.Constants.Permission;
using Coworkee.Shared.Wrapper;
using HeyRed.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Coworkee.Server.Controllers.Catalog
{
    public class ProductsController : BaseApiController<ProductsController>
    {
        /// <summary>
        /// Get All Products
        /// </summary>
        /// <returns>Status 200 OK</returns>
        // [Authorize(Policy = Permissions.Products.View)]
        [Filters.CustomAuthorize(Policies = new[] { Permissions.Products.View })]
        [HttpGet]
        [Produces(typeof(PaginatedResult<ProductDto>))]
        public async Task<IActionResult> GetAll([FromQuery] GetAllProductsQuery query, CancellationToken cancellationToken = default)
        {
            return Ok(await Mediator.Send(query, cancellationToken));
        }

        /// <summary>
        /// Get a Product Image by Id
        /// </summary>
        /// <param name="id">Product Id</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Products.View)]
        [HttpGet("image/{id}")]
        [Produces(typeof(Result<string>))]
        public async Task<IActionResult> GetProductImageAsync(string id, CancellationToken cancellationToken = default)
        {
            var result = await Mediator.Send(new GetProductImageQuery(UnhashId(id)), cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get a Product by Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Status 200 Ok</returns>
        [Filters.CustomAuthorize(Policies = new[] { Permissions.Products.View })]
        [HttpGet("{id}")]
        [Produces(typeof(ProductDto))]
        public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken = default)
        {
            var product = await Mediator.Send(new GetProductByIdQuery(UnhashId(id)), cancellationToken);
            return Ok(product);
        }

        /// <summary>
        /// Add/Edit a Product
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Status 200 OK</returns>
        [Filters.CustomAuthorize(Policies = new[] { Permissions.Products.Create, Permissions.Products.Edit }, PolicyMatch = PolicyMatch.Any)]
        [HttpPost]
        public async Task<IActionResult> Post(AddEditProductsCommand command, CancellationToken cancellationToken = default)
        {
            return Ok(await Mediator.Send(command, cancellationToken));
        }

        /// <summary>
        /// Delete a Product
        /// </summary>
        /// <param name="ids">Products to delete</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Status 200 OK response</returns>
        [Authorize(Policy = Permissions.Products.Delete)]
        [HttpDelete]
        public async Task<IActionResult> Delete(string[] ids, CancellationToken cancellationToken = default)
        {
            return Ok(await Mediator.Send(new DeleteProductCommand { Ids = UnhashIds(ids) }, cancellationToken));
        }
        
        /// <summary>
        /// Exports products
        /// </summary>
        /// <returns></returns>
        [Authorize(Policy = Permissions.Products.Export)]
        [HttpGet(nameof(Export))]
        [AllowSynchronousIo]
        public async Task<IActionResult> Export([FromQuery]ExportProductsQuery query, CancellationToken cancellationToken = default)
        {
            var res = await Mediator.Send(query, cancellationToken);
            var mimeType = MimeGuesser.GuessMimeType(res);
            var fileDownloadName = $"{ControllerName}-{DateTime.Now:ddMMyyyyHHmmss}.{MimeTypesMap.GetExtension(mimeType)}";
            return File(res, mimeType, fileDownloadName);
        }
    }
}