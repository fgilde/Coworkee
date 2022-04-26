using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Common.Security;
using CleanArchitectureBase.Application.Contracts.Repositories;
using CleanArchitectureBase.Application.Contracts.Services;
using CleanArchitectureBase.Application.Features.Base.Queries;
using CleanArchitectureBase.Domain.Entities.Notifications;
using CleanArchitectureBase.Shared.Constants.Role;
using Microsoft.Extensions.Localization;
using Nextended.Core.Extensions;

namespace CleanArchitectureBase.Application.Features.Notifications.Queries.GetById
{
    [CustomAuthorize]
    public class GetNotificationByIdQuery : GetByIdQueryBase<int, NotificationDto>
    {
        public GetNotificationByIdQuery(int id) : base(id)
        { }
    }

    internal class GetNotificationByIdQueryHandler : GetByIdQueryHandlerBase<GetNotificationByIdQuery, int, NotificationDto, Notification>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IStringLocalizer<GetNotificationByIdQueryHandler> _localizer;

        public GetNotificationByIdQueryHandler(IUnitOfWork<int> unitOfWork,
            IServiceProvider provider, 
            ICurrentUserService currentUserService, 
            IStringLocalizer<GetNotificationByIdQueryHandler> localizer) 
            : base(unitOfWork, provider)
        {
            _currentUserService = currentUserService;
            _localizer = localizer;
        }

        public override async Task<NotificationDto> Handle(GetNotificationByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await UnitOfWork.Repository<Notification>().GetByIdAsync(request.Id, cancellationToken);
            if (result == null || (result.UserId != _currentUserService.UserId && !_currentUserService.Principal.IsInRole(RoleConstants.AdministratorRole)))
                throw Errors.NotFound(_localizer["Notification Not Found!"]);
            return result.MapTo<NotificationDto>();
        }
    }
}