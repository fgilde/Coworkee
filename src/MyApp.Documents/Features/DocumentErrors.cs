using Coworkee.Core.Results;

namespace MyApp.Documents.Features;

internal static class DocumentErrors
{
    public static readonly Error NotFound = Error.NotFound("documents.not_found", "The document does not exist.");

    public static readonly Error TypeNotFound = Error.NotFound("documents.type_not_found", "The document type does not exist.");

    public static readonly Error TypeExists = Error.Conflict("documents.type_exists", "A document type with this name exists.");

    public static readonly Error UnknownType = Error.Validation("DocumentTypeId", "Choose an existing document type.");

    public static readonly Error NoTenant = Error.Forbidden("documents.no_tenant", "Documents belong to an organisation.");

    public static readonly Error Forbidden = Error.Forbidden("documents.forbidden", "You may not do this.");
}
