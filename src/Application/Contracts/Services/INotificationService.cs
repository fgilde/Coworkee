using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Requests;
using CleanArchitectureBase.Domain.Entities.Notifications;

namespace CleanArchitectureBase.Application.Contracts.Services;

public interface INotificationService
{
    Task SendAsync(NotificationRequest request, CancellationToken cancellationToken = default);
}