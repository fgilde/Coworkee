using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Features.Brands.Commands.AddEdit;
using CleanArchitectureBase.Application.Features.Brands.Commands.Delete;
using CleanArchitectureBase.Application.Features.Brands.Queries.Export;
using CleanArchitectureBase.Application.Features.Brands.Queries.GetAll;
using CleanArchitectureBase.Application.Features.Brands.Queries.GetById;
using CleanArchitectureBase.Shared.Constants.Permission;
using CleanArchitectureBase.Shared.Wrapper;
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
        [Authorize(Policy = Permissions.Brands.View)]
        [HttpGet]
        [Produces(typeof(Result<List<GetAllBrandsResponse>>))]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
        {
            var brands = await Mediator.Send(new GetAllBrandsQuery(), cancellationToken);
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
        [Produces(typeof(Result<GetBrandByIdResponse>))]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
        {
            var brand = await Mediator.Send(new GetBrandByIdQuery { Id = id }, cancellationToken);
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
        [Produces(typeof(Result<int>))]
        public async Task<IActionResult> Post(AddEditBrandCommand command, CancellationToken cancellationToken = default)
        {
            return Ok(await Mediator.Send(command, cancellationToken));
        }

        /// <summary>
        /// Delete a Brand
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Brands.Delete)]
        [HttpDelete("{id}")]
        [Produces(typeof(Result<int>))]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            return Ok(await Mediator.Send(new DeleteBrandCommand { Id = id }, cancellationToken));
        }

        /// <summary>
        /// Search Brands and Export to Excel
        /// </summary>
        /// <param name="searchString"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Authorize(Policy = Permissions.Brands.Export)]
        [HttpGet("export")]
        [Produces(typeof(Result<string>))]
        public async Task<IActionResult> Export(string searchString = "", CancellationToken cancellationToken = default)
        {
            return Ok(await Mediator.Send(new ExportBrandsQuery(searchString), cancellationToken));
        }
    }
}