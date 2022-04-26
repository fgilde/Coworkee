using System;
using System.Collections.Generic;
using System.Linq;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Common.Security;
using CleanArchitectureBase.Application.Contracts.Repositories;
using CleanArchitectureBase.Application.Contracts.Services;
using CleanArchitectureBase.Application.Features.Base.Commands;
using CleanArchitectureBase.Domain.Entities.Notifications;
using CleanArchitectureBase.Shared.Constants.Role;

namespace CleanArchitectureBase.Application.Features.Notifications.Commands.Delete
{
    [CustomAuthorize]
    public class DeleteNotificationsCommand : DeleteCommandBase<int>
    {
        public bool All { get; set; }
    }

    internal class DeleteDocumentsCommandHandler : DeleteCommandHandlerBase<DeleteNotificationsCommand, int, NotificationDto, Notification>
    {
        private readonly ICurrentUserService _currentUserService;

        public DeleteDocumentsCommandHandler(IUnitOfWork<int> unitOfWork, IMediator mediator, IPermissionService permissionService, IServiceProvider provider, ICurrentUserService currentUserService)
            : base(unitOfWork, mediator, permissionService, provider)
        {
            _currentUserService = currentUserService;
        }

        protected override async Task<IEnumerable<Notification>> FindEntitiesAsync(DeleteNotificationsCommand command, CancellationToken cancellationToken)
        {
            if (command.All)
                return UnitOfWork.Repository<Notification>().Entities.Where(n => n.UserId == _currentUserService.UserId).AsEnumerable();
            
            var entities = await base.FindEntitiesAsync(command, cancellationToken);
            return _currentUserService.Principal.IsInRole(RoleConstants.AdministratorRole) 
                ? entities 
                : entities.Where(n => n.UserId == _currentUserService.UserId);
        }
    }
}