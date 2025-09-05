using System;
using System.Linq;
using System.Threading.Tasks;
using lib.Coworkee.Application.Features.Dashboards.Queries.GetData;
using Coworkee.Infrastructure.Contexts;
using Grpc.Core;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Nextended.Core.Extensions;

// Dont change this namespace it will be partial on client generator
namespace Coworkee.Data;

[Authorize]
public class MainDataService : CoworkeeData.CoworkeeDataBase
{
    private readonly ApplicationDbContext _db;
    private readonly IMediator _mediator;

    public MainDataService(ApplicationDbContext db, IMediator mediator)
    {
        this._db = db;
        _mediator = mediator;
    }

    public override async Task<DashboardReply> GetDashboardData(DashboardRequest request, ServerCallContext context)
    {
        var result = await _mediator.Send(new GetDashboardDataQuery());
        return result.Data.MapTo<DashboardReply>();
    }

    public override async Task<ProductsReply> GetProducts(ProductsRequest request, ServerCallContext context)
    {
        var modifiedParts = _db.Products
            .OrderBy(p => p.LastModifiedOn)
            .Where(p => p.LastModifiedOn.Value.Ticks > request.ModifiedSince);
         //
        var reply = new ProductsReply();
        reply.ModifiedCount = await modifiedParts.CountAsync();
      //  reply.Products.AddRange(await modifiedParts.Take(request.MaxCount).ToListAsync());
        return reply;
    }
}
