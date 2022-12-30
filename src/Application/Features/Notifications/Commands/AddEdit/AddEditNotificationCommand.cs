using System;
using MediatR;
using Coworkee.Application.Common.Models;
using Coworkee.Application.Common.Security;
using Coworkee.Application.Contracts.Repositories;
using Coworkee.Application.Contracts.Services;
using Coworkee.Application.Features.Base.Commands;
using Coworkee.Domain.Entities.Notifications;

namespace Coworkee.Application.Features.Notifications.Commands.AddEdit
{
    [CustomAuthorize]
    public class AddEditNotificationCommand : AddEditCommandBase<NotificationDto>
    {
        public AddEditNotificationCommand(params NotificationDto[] items) : base(items)
        { }
    }

    internal class AddEditNotificationCommandHandler : AddEditCommandHandlerBase<AddEditNotificationCommand, int, NotificationDto, Notification>
    {
        public AddEditNotificationCommandHandler(IUnitOfWork<int> unitOfWork, IMediator mediator, IPermissionService permissionService, IServiceProvider provider)
            : base(unitOfWork, mediator, permissionService, provider)
        { }
    }
}