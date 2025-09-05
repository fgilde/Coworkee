using MediatR;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Coworkee.Application.Common.Models;
using Coworkee.Application.Contracts.Repositories;
using Coworkee.Application.Contracts.Services.ExportImport;
using Coworkee.Application.Features.Base.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Coworkee.Application.Features.Base.Import;

public class ImportQueryBase<TDto> : IRequest<AddUpdateResult<TDto>> 
    where TDto : IDtoBase
{
    public string ContentType { get; set; }
    public byte[] Data { get; set; }
}

internal abstract class ImportQueryHandlerBase<TQuery, TEntityId, TDto, TAddEditCommand> : IRequestHandler<TQuery, AddUpdateResult<TDto>>
    where TQuery : ImportQueryBase<TDto>
    where TAddEditCommand: AddEditCommandBase<TDto>, new()
    where TDto : class, IDtoBase<TEntityId>
{
    protected readonly IUnitOfWork<TEntityId> UnitOfWork;
    protected readonly IStringLocalizer Localizer;
    protected readonly IServiceProvider Provider;
    protected T Get<T>() => Provider.GetService<T>();

    protected ImportQueryHandlerBase(IUnitOfWork<TEntityId> unitOfWork, IStringLocalizer localizer, IServiceProvider serviceProvider)
    {
        UnitOfWork = unitOfWork;
        Localizer = localizer;
        Provider = serviceProvider;
    }

    protected virtual IImportService GetImportService(TQuery query)
    {
        return Provider.GetServices<IImportService>()
            .FirstOrDefault(s => s.SupportedContentTypes.Contains(query.ContentType, StringComparer.InvariantCultureIgnoreCase));
    }

    public virtual async Task<AddUpdateResult<TDto>> Handle(TQuery request, CancellationToken cancellationToken)
    {
        var service = GetImportService(request);
        if (service == null)
            throw Errors.Create("ContentType '{0}' not supported by any Importer", HttpStatusCode.UnprocessableEntity, request.ContentType);
        var items = (await service.ImportAsync<TDto>(request.Data, cancellationToken)).ToArray();
        return await Get<IMediator>().Send(new TAddEditCommand
        {
            Items = items
        }, cancellationToken);
    }
}