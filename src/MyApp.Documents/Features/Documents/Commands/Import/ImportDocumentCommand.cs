using Coworkee.Application.Authorization;
using Coworkee.Application.Messaging;
using Coworkee.Core.Results;
using MyApp.Contracts.Documents;

namespace MyApp.Documents.Features.Documents.Commands.Import;

[RequiresPermission(DocumentPermissions.Documents.Create)]
public sealed record ImportDocumentCommand(ImportDocumentRequest Request) : ICommand<Result<DocumentDto>>;
