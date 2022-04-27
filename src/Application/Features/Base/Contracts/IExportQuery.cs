using CleanArchitectureBase.Application.Contracts.Enums;
using MediatR;

namespace CleanArchitectureBase.Application.Features.Base.Contracts;

public interface IExportQuery<out TEntityId> : IRequest<byte[]>
{
    ExportServiceType ExportServiceType { get; }
    string SearchString { get; }
    TEntityId[] Ids { get; }
}