using Coworkee.Application.Authorization;
using Coworkee.Application.Messaging;
using Coworkee.Core.Results;
using MyApp.Contracts.Documents;

namespace MyApp.Documents.Features.Documents.Queries.Open;

[AiTool(Exclude = true)]
[RequiresPermission(DocumentPermissions.Documents.View)]
public sealed record OpenDocumentQuery(Guid Id) : IQuery<Result<DocumentContent>>;

public sealed record DocumentContent(Stream Content, string FileName, string MimeType);
