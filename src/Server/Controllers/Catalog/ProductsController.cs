using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Dtos;
using CleanArchitectureBase.Application.Features.Products.Commands.AddEdit;
using CleanArchitectureBase.Application.Features.Products.Commands.Delete;
using CleanArchitectureBase.Application.Features.Products.Queries.Export;
using CleanArchitectureBase.Application.Features.Products.Queries.GetAllPaged;
using CleanArchitectureBase.Application.Features.Products.Queries.GetById;
using CleanArchitectureBase.Application.Features.Products.Queries.GetProductImage;
using CleanArchitectureBase.Application.Security;
using CleanArchitectureBase.Shared.Constants.Permission;
using CleanArchitectureBase.Shared.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitectureBase.Server.Controllers.Catalog
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
        public async Task<IActionResult> GetProductImageAsync(int id, CancellationToken cancellationToken = default)
        {
            var result = await Mediator.Send(new GetProductImageQuery(id), cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get a Brand By Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Status 200 Ok</returns>
        [Filters.CustomAuthorize(Policies = new[] { Permissions.Products.View })]
        [HttpGet("{id}")]
        [Produces(typeof(ProductDto))]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
        {
            var product = await Mediator.Send(new GetProductByIdQuery(id), cancellationToken);
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
        public async Task<IActionResult> Delete(int[] ids, CancellationToken cancellationToken = default)
        {
            return Ok(await Mediator.Send(new DeleteProductCommand { Ids = ids }, cancellationToken));
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
            return Ok(await Mediator.Send(new ExportProductsQuery(searchString), cancellationToken));
        }

        /// <summary>
        /// Exports specific products as excel
        /// </summary>
        /// <param name="ids">Produc ids to export</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Products.Export)]
        [HttpGet("exportByIds")]
        [Produces(typeof(Result<string>))]
        public async Task<IActionResult> ExportByIds([FromQuery] int[] ids, CancellationToken cancellationToken = default)
        {
            return Ok(await Mediator.Send(new ExportProductsQuery(ids), cancellationToken));
        }
    }
}