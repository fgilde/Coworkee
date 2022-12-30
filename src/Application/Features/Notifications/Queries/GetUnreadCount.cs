using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Coworkee.Application.Contracts.Repositories;
using Coworkee.Application.Contracts.Services;
using Coworkee.Domain.Entities.Notifications;
using MediatR;

namespace Coworkee.Application.Features.Notifications.Queries
{
    public class GetUnreadCount
    {
        public class Request : IRequest<int> { }

        internal class Handler : IRequestHandler<Request, int>
        {
            private readonly ICurrentUserService _currentUserService;
            private readonly IUnitOfWork<int> _unitOfWork;

            public Handler(ICurrentUserService currentUserService, IUnitOfWork<int> unitOfWork)
            {
                _currentUserService = currentUserService;
                _unitOfWork = unitOfWork;
            }

            public Task<int> Handle(Request request, CancellationToken cancellationToken)
            {
                var unread = _unitOfWork.Repository<Notification>().Entities.Where(n => n.UserId == _currentUserService.UserId && !n.IsRead).Count();
                return Task.FromResult(unread);
            }
        }
    }
}
