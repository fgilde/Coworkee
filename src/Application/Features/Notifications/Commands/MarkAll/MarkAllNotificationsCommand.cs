using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using lib.Coworkee.Application.Contracts.Repositories;
using lib.Coworkee.Application.Contracts.Services;
using lib.Coworkee.Domain.Entities.Notifications;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Coworkee.Application.Features.Notifications.Commands.MarkAll;


public class MarkAllNotificationsCommand
{
    public class Request : IRequest
    {
        public bool IsRead { get; set; }
    }

    internal class Handler : IRequestHandler<Request>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork<int> _unitOfWork;

        public Handler(ICurrentUserService currentUserService, IUnitOfWork<int> unitOfWork)
        {
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(Request request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.Repository<Notification>();
            var notifications = repo.Entities.Where(n => n.UserId == _currentUserService.UserId);

            await notifications.ForEachAsync(a => a.IsRead = request.IsRead, cancellationToken);
            await _unitOfWork.Commit(cancellationToken);

           // await repo.UpdateManyAsync(toUpdate.Select(t => t.Entity), cancellationToken);
        }
    }
}