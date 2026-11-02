using Coworkee.Application.Authorization;
using Coworkee.Application.Messaging;
using Coworkee.Core.Results;
using Coworkee.Infrastructure.Persistence;
using Coworkee.OData.ClientEntities;
using Microsoft.EntityFrameworkCore;
using MyApp.Contracts.Documents;
using MyApp.Documents.Domain;

namespace MyApp.Documents.Features.DocumentTypes.Commands.AddEdit;

internal sealed class AddEditDocumentTypeHandler(CoworkeeDbContext db, IPermissionChecker permissions, IClientEntityChangeLog changeLog) : IHandler<AddEditDocumentTypeCommand, Result<DocumentTypeDto>>
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

        if (command.Id is not null && type.Name != name)
        {
            // the client copies of documents expand their type: a new name moves no ModifiedAt of theirs
            changeLog.Touch<Document>(await db.Set<Document>().Where(d => d.DocumentTypeId == type.Id).Select(d => d.Id).ToListAsync(cancellationToken));
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
