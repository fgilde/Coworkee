using System.ComponentModel;
using Coworkee.Application.Authorization;
using Coworkee.Application.Messaging;
using Coworkee.Core.Results;
using MyApp.Contracts.Documents;

namespace MyApp.Documents.Features.Documents.Commands.Update;

[Description("Changes title, description, type or visibility of a document.")]
[RequiresPermission(DocumentPermissions.Documents.Edit)]
public sealed record UpdateDocumentCommand(Guid Id, UpdateDocumentRequest Document) : ICommand<Result<DocumentDto>>;
