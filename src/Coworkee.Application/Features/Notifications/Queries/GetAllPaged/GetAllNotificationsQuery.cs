using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Coworkee.Application.Common.Models;
using Coworkee.Application.Common.Security;
using Coworkee.Application.Contracts.Repositories;
using Coworkee.Application.Contracts.Services;
using Coworkee.Application.Features.Base.Queries;
using Coworkee.Application.Specifications.Base;
using Coworkee.Application.Specifications.Notifications;
using Coworkee.Domain.Entities.Notifications;
using Coworkee.Shared.Wrapper;
using MediatR;

namespace Coworkee.Application.Features.Notifications.Queries.GetAllPaged;

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