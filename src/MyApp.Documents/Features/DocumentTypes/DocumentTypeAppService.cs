using Coworkee.Application.Authorization;
using Coworkee.Application.Messaging;
using Coworkee.Core.Results;
using Coworkee.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MyApp.Contracts;
using MyApp.Contracts.Documents;
using MyApp.Documents.Domain;
using MyApp.Documents.Features.DocumentTypes.Commands.AddEdit;
using MyApp.Documents.Features.DocumentTypes.Commands.Delete;

namespace MyApp.Documents.Features.DocumentTypes;

internal sealed class DocumentTypeAppService(IDispatcher dispatcher, CoworkeeDbContext db) : IDocumentTypeAppService
{
    [RequiresPermission(DocumentPermissions.Types.View)]
    public async Task<DocumentTypeDto> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
        (await db.Set<DocumentType>().AsNoTracking().SingleOrDefaultAsync(t => t.Id == id, cancellationToken))?.ToDto()
        ?? throw new ErrorException(DocumentErrors.TypeNotFound);

    public Task<DocumentTypeDto> CreateAsync(AddEditDocumentTypeRequest input, CancellationToken cancellationToken = default) =>
        dispatcher.SendAsync(new AddEditDocumentTypeCommand(null, input), cancellationToken).OrThrow();

    public Task<DocumentTypeDto> UpdateAsync(Guid id, AddEditDocumentTypeRequest input, CancellationToken cancellationToken = default) =>
        dispatcher.SendAsync(new AddEditDocumentTypeCommand(id, input), cancellationToken).OrThrow();

    public Task DeleteAsync(IdsRequest request, CancellationToken cancellationToken = default) =>
        dispatcher.SendAsync(new DeleteDocumentTypesCommand(request.Ids), cancellationToken).OrThrow();
}
