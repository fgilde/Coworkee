using System.ComponentModel;
using Coworkee.Application.Authorization;
using Coworkee.Application.Messaging;
using Coworkee.Core.Results;
using MyApp.Contracts.Documents;

namespace MyApp.Documents.Features.DocumentTypes.Commands.Delete;

[Description("Deletes document types by id.")]
[RequiresPermission(DocumentPermissions.Types.Delete)]
public sealed record DeleteDocumentTypesCommand(IReadOnlyList<Guid> Ids) : ICommand<Result>;
