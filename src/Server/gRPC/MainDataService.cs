using System;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Features.Dashboards.Queries.GetData;
using CleanArchitectureBase.Infrastructure.Contexts;
using Grpc.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

// Dont change this namespace it will be partial on client generator
namespace CleanArchitectureBase.Data;

//[Authorize]
public class MainDataService : CleanArchitectureBaseData.CleanArchitectureBaseDataBase
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
        //var dr = await _mediator.Send(new GetDashboardDataQuery());
        return new DashboardReply
        {
            ProjectsBookedValue = 38_000_000,
            NextDeliveryDueInMs = (long)TimeSpan.FromHours(53).TotalMilliseconds,
            StaffOnSite = 441,
            FactoryUptimeMs = (long)TimeSpan.FromDays(152).TotalMilliseconds,
            ServicingTasksDue = 7,
            MachinesStopped = 3,
        };
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
