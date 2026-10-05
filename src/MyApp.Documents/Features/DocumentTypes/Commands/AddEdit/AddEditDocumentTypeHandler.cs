using Coworkee.Application.Authorization;
using Coworkee.Application.Messaging;
using Coworkee.Core.Results;
using Coworkee.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MyApp.Contracts.Documents;
using MyApp.Documents.Domain;

namespace MyApp.Documents.Features.DocumentTypes.Commands.AddEdit;

internal sealed class AddEditDocumentTypeHandler(CoworkeeDbContext db, IPermissionChecker permissions) : IHandler<AddEditDocumentTypeCommand, Result<DocumentTypeDto>>
{
    public async Task<Result<DocumentTypeDto>> HandleAsync(AddEditDocumentTypeCommand command, CancellationToken cancellationToken)
    {
        var required = command.Id is null ? DocumentPermissions.Types.Create : DocumentPermissions.Types.Edit;
        if (!await permissions.IsGrantedAsync(required, cancellationToken))
        {
            return DocumentErrors.Forbidden;
        }

        var name = command.Type.Name.Trim();
        if (await db.Set<DocumentType>().AnyAsync(t => t.Name == name && t.Id != command.Id, cancellationToken))
        {
            return DocumentErrors.TypeExists;
        }

        var type = command.Id is { } id ? await db.Set<DocumentType>().SingleOrDefaultAsync(t => t.Id == id, cancellationToken) : Add(name);
        if (type is null)
        {
            return DocumentErrors.TypeNotFound;
        }

        type.Name = name;
        type.Description = string.IsNullOrWhiteSpace(command.Type.Description) ? null : command.Type.Description.Trim();
        return type.ToDto();
    }

    private DocumentType Add(string name)
    {
        var type = new DocumentType { Name = name };
        db.Add(type);
        return type;
    }
}
