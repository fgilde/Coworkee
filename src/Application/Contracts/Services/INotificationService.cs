using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using lib.Coworkee.Application.Requests;
using lib.Coworkee.Domain.Entities.Notifications;

namespace Coworkee.Application.Contracts.Services;

public interface INotificationService
{
    Task SendAsync(NotificationRequest request, CancellationToken cancellationToken = default);
}