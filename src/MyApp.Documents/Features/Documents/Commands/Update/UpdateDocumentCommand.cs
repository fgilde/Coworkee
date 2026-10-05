using Coworkee.Application.Authorization;
using Coworkee.Application.Messaging;
using Coworkee.Core.Results;
using MyApp.Contracts.Documents;

namespace MyApp.Documents.Features.Documents.Commands.Update;

[RequiresPermission(DocumentPermissions.Documents.Edit)]
public sealed record UpdateDocumentCommand(Guid Id, UpdateDocumentRequest Document) : ICommand<Result<DocumentDto>>;
