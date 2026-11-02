using Coworkee.Application.Messaging;
using Coworkee.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyApp.Catalog.Features.Products.Queries.GetByBarcode;
using MyApp.Contracts.Catalog;

namespace MyApp.Catalog.Endpoints;

/// <summary>An MVC controller in a module: UseCoworkee finds and maps it. It sends requests like the minimal APIs do.</summary>
[ApiController]
[Authorize]
[Route("api/v1/product-lookup")]
[Tags("Products")]
public sealed class ProductLookupController(IDispatcher dispatcher) : ControllerBase
{
    [HttpGet("by-barcode/{barcode}")]
    public Task<Results<Ok<ProductDto>, ProblemHttpResult, ValidationProblem>> ByBarcode(string barcode, CancellationToken cancellationToken) =>
        dispatcher.SendAsync(new GetProductByBarcodeQuery(barcode), cancellationToken).ToHttpResult();
}
