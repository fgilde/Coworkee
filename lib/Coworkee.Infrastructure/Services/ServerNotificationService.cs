using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Coworkee.Application.Common.Extensions;
using Coworkee.Application.Common.Models.Identity;
using Coworkee.Application.Contracts.Attributes;
using Coworkee.Application.Contracts.Repositories;
using Coworkee.Application.Contracts.Services;
using Coworkee.Application.Contracts.Services.Identity;
using Coworkee.Application.Hubs.Events.Base;
using Coworkee.Application.Requests;
using Coworkee.Domain.Entities.Notifications;
using Hangfire;
using MediatR;
using Nextended.Core.Attributes;
using Nextended.Core.Extensions;

namespace Coworkee.Infrastructure.Services
{
    [RegisterAs(typeof(INotificationService))]
    public class ServerNotificationService : INotificationService
    {
        private readonly IUserService _userService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMediator _mediator;
        private readonly IMailService _mailService;

        public ServerNotificationService(IUserService userService,
            ICurrentUserService currentUserService,
            IUnitOfWork<int> unitOfWork,
            IMediator mediator,
            IMailService mailService)
        {
            _userService = userService;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _mediator = mediator;
            _mailService = mailService;
        }


        public async Task SendAsync(NotificationRequest request, CancellationToken cancellationToken = default)
        {
            var users = (await _userService.GetAllForTargetAsync(request.Target ?? EventTarget.All))
                .Where(u => !request.SkipCurrentUser || string.IsNullOrEmpty(_currentUserService.UserId) || u.Id != _currentUserService.UserId ).ToList();
            
            if (request.SendAsMail != NotificationAsMail.Never)
            {
                var mailReceivers = request.SendAsMail == NotificationAsMail.Always ? users : users.Where(IsOffline);
                mailReceivers.Apply(user => BackgroundJob.Enqueue(() => _mailService.SendAsync(request.ToMailRequest(user))));
            }
            if (request.PersistInDb)
            {
                await _unitOfWork.Repository<Notification>().AddManyAsync(users.Select(u => DbNotification(u, request)), cancellationToken);
                await _unitOfWork.Commit(cancellationToken);
            }
            await _mediator.PublishClientEvent(request, cancellationToken);
        }
        
        private Notification DbNotification(UserResponse user, NotificationRequest request)
        {
            return new Notification
            {
                UserId = user.Id,
                NotificationTypeId = request.NotificationTypeId,
                Content = request.Content,
                Excerpt = request.Excerpt,
                Subject = request.Subject,
                Url = request.Url,
                IsRead = false,
                SentAsMail = request.SendAsMail == NotificationAsMail.Always || (request.SendAsMail == NotificationAsMail.WhenTargetOffline && IsOffline(user)),
            };
        }

        private bool IsOffline(UserResponse user)
        {
            return !user.IsOnline;
        }
    }
}