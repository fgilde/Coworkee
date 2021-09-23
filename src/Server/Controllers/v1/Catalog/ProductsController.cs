using System.Threading;
using CleanArchitectureBase.Application.Features.Products.Commands.AddEdit;
using CleanArchitectureBase.Application.Features.Products.Commands.Delete;
using CleanArchitectureBase.Application.Features.Products.Queries.Export;
using CleanArchitectureBase.Application.Features.Products.Queries.GetAllPaged;
using CleanArchitectureBase.Application.Features.Products.Queries.GetProductImage;
using CleanArchitectureBase.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CleanArchitectureBase.Shared.Wrapper;

namespace CleanArchitectureBase.Server.Controllers.v1.Catalog
{
    public class ProductsController : BaseApiController<ProductsController>
    {
        /// <summary>
        /// Get All Products
        /// </summary>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Products.View)]
        [HttpGet]
        [Produces(typeof(PaginatedResult<GetAllPagedProductsResponse>))]
        public async Task<IActionResult> GetAll([FromQuery] GetAllProductsQuery query, CancellationToken cancellationToken = default)
        {
            return Ok(await _mediator.Send(query, cancellationToken));
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
        public async Task<IActionResult> GetProductImageAsync(int id, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetProductImageQuery(id), cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Add/Edit a Product
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Products.Create)]
        [HttpPost]
        [Produces(typeof(Result<int>))]
        public async Task<IActionResult> Post(AddEditProductCommand command, CancellationToken cancellationToken = default)
        {
            return Ok(await _mediator.Send(command, cancellationToken));
        }

        /// <summary>
        /// Delete a Product
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Status 200 OK response</returns>
        [Authorize(Policy = Permissions.Products.Delete)]
        [HttpDelete("{id}")]
        [Produces(typeof(Result<int>))]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            return Ok(await _mediator.Send(new DeleteProductCommand { Id = id }, cancellationToken));
        }

        /// <summary>
        /// Search Products and Export to Excel
        /// </summary>
        /// <param name="searchString"></param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Products.Export)]
        [HttpGet("export")]
        [Produces(typeof(Result<string>))]
        public async Task<IActionResult> Export(string searchString = "", CancellationToken cancellationToken = default)
        {
            return Ok(await _mediator.Send(new ExportProductsQuery(searchString), cancellationToken));
        }
    }
}