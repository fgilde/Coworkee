using System.ComponentModel;
using Coworkee.Application.Authorization;
using Coworkee.Application.Messaging;
using Coworkee.Core.Results;
using MyApp.Contracts.Documents;

namespace MyApp.Documents.Features.Documents.Queries.GetById;

[Description("Reads one document by id.")]
[RequiresPermission(DocumentPermissions.Documents.View)]
public sealed record GetDocumentByIdQuery(Guid Id) : IQuery<Result<DocumentDto>>;
