using System;
using MediatR;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Common.Security;
using CleanArchitectureBase.Application.Contracts.Repositories;
using CleanArchitectureBase.Application.Contracts.Services;
using CleanArchitectureBase.Application.Features.Base.Commands;
using CleanArchitectureBase.Domain.Entities.Notifications;

namespace CleanArchitectureBase.Application.Features.Notifications.Commands.AddEdit
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