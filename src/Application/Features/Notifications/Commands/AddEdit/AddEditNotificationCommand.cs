using System;
using MediatR;
using lib.Coworkee.Application.Common.Models;
using lib.Coworkee.Application.Common.Security;
using lib.Coworkee.Application.Contracts.Repositories;
using lib.Coworkee.Application.Contracts.Services;
using lib.Coworkee.Application.Features.Base.Commands;
using lib.Coworkee.Domain.Entities.Notifications;

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