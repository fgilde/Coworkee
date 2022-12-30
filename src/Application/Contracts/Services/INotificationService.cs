using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Coworkee.Application.Requests;
using Coworkee.Domain.Entities.Notifications;

namespace Coworkee.Application.Contracts.Services;

public interface INotificationService
{
    Task SendAsync(NotificationRequest request, CancellationToken cancellationToken = default);
}