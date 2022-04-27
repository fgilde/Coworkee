using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Common.Security;
using CleanArchitectureBase.Application.Contracts.Repositories;
using CleanArchitectureBase.Application.Contracts.Services;
using CleanArchitectureBase.Application.Features.Base.Queries;
using CleanArchitectureBase.Application.Specifications.Base;
using CleanArchitectureBase.Application.Specifications.Notifications;
using CleanArchitectureBase.Domain.Entities.Notifications;
using CleanArchitectureBase.Shared.Wrapper;
using MediatR;

namespace CleanArchitectureBase.Application.Features.Notifications.Queries.GetAllPaged;

[CustomAuthorize]
public class GetAllNotificationsQuery : GetAllPagedQueryBase<NotificationDto>
{
    public bool UnreadOnly { get; set; }
    public string NotificationTypeId { get; set; }
}

internal class GetAllNotificationsQueryHandler : GetAllPagedQueryHandlerBase<GetAllNotificationsQuery, int, NotificationDto, Notification>
{
    private readonly ICurrentUserService _currentUserService;

    protected override ISpecification<Notification> GetFilterSpecification(GetAllNotificationsQuery query)
    {
        return new NotificationFilterSpecification(query.SearchString, _currentUserService.UserId, query);
    }

    public GetAllNotificationsQueryHandler(IUnitOfWork<int> unitOfWork, IMediator mediator, IServiceProvider provider, ICurrentUserService currentUserService)
        : base(unitOfWork, mediator, provider)
    {
        _currentUserService = currentUserService;
    }

    public override Task<PaginatedResult<NotificationDto>> Handle(GetAllNotificationsQuery request, CancellationToken cancellationToken)
    {
        if (request.OrderBy is not { Length: > 0 })
            request.OrderBy = new[] { $"{nameof(Notification.IsRead)} Ascending", $"{nameof(Notification.CreatedOn)} Descending" };
        return base.Handle(request, cancellationToken);
    }
}