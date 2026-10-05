using Coworkee.Application.Authorization;
using Coworkee.Application.Messaging;
using Coworkee.Core.Results;
using MyApp.Contracts.Documents;

namespace MyApp.Documents.Features.Documents.Commands.Delete;

[RequiresPermission(DocumentPermissions.Documents.Delete)]
public sealed record DeleteDocumentsCommand(IReadOnlyList<Guid> Ids) : ICommand<Result>;
