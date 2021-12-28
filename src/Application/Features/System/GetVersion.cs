using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Common.Models;
using MediatR;

namespace CleanArchitectureBase.Application.Features.System
{
    public class GetVersion
    {
        public class Request : IRequest<VersionInfoModel> { }

        internal class Handler : IRequestHandler<Request, VersionInfoModel>
        {
            public Task<VersionInfoModel> Handle(Request request, CancellationToken cancellationToken)
            {
                return Task.FromResult(new VersionInfoModel());
            }
        }
    }
}
