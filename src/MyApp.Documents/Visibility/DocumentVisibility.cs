using Coworkee.Application.Authorization;
using Coworkee.Core.Security;
using MyApp.Contracts.Documents;
using MyApp.Documents.Domain;

namespace MyApp.Documents.Visibility;

internal sealed class DocumentVisibility(ICurrentUser currentUser, IPermissionChecker permissions)
{
    public async Task<IQueryable<Document>> VisibleAsync(IQueryable<Document> documents, CancellationToken cancellationToken)
    {
        if (await permissions.IsGrantedAsync(DocumentPermissions.Documents.ManageAll, cancellationToken))
        {
            return documents;
        }

        var userId = currentUser.UserId;
        return documents.Where(d => d.IsPublic || d.OwnerId == userId);
    }

    public async Task<IQueryable<Document>> EditableAsync(IQueryable<Document> documents, CancellationToken cancellationToken)
    {
        if (await permissions.IsGrantedAsync(DocumentPermissions.Documents.ManageAll, cancellationToken))
        {
            return documents;
        }

        var userId = currentUser.UserId;
        return documents.Where(d => d.OwnerId == userId);
    }
}
