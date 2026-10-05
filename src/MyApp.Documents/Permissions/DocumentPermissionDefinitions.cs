using Coworkee.Application.Authorization;
using MyApp.Contracts.Documents;

namespace MyApp.Documents.Permissions;

internal sealed class DocumentPermissionDefinitions : IPermissionDefinitionContributor
{
    public void Define(PermissionDefinitionContext context) =>
        context.Group(DocumentPermissions.GroupName, "Documents")
            .Add(DocumentPermissions.Documents.View, "View public and own documents")
            .Add(DocumentPermissions.Documents.Create, "Upload documents", DocumentPermissions.Documents.View)
            .Add(DocumentPermissions.Documents.Edit, "Edit own documents", DocumentPermissions.Documents.View)
            .Add(DocumentPermissions.Documents.Delete, "Delete own documents", DocumentPermissions.Documents.View)
            .Add(DocumentPermissions.Documents.Export, "Export documents", DocumentPermissions.Documents.View)
            .Add(DocumentPermissions.Documents.ManageAll, "View, edit and delete every document",
                DocumentPermissions.Documents.Edit, DocumentPermissions.Documents.Delete)
            .Add(DocumentPermissions.Types.View, "View document types")
            .Add(DocumentPermissions.Types.Create, "Create document types", DocumentPermissions.Types.View)
            .Add(DocumentPermissions.Types.Edit, "Edit document types", DocumentPermissions.Types.View)
            .Add(DocumentPermissions.Types.Delete, "Delete document types", DocumentPermissions.Types.View);
}
