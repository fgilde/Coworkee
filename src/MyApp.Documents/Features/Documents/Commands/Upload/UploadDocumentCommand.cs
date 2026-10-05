using Coworkee.Application.Authorization;
using Coworkee.Application.Messaging;
using Coworkee.Core.Results;
using MyApp.Contracts.Documents;

namespace MyApp.Documents.Features.Documents.Commands.Upload;

[RequiresPermission(DocumentPermissions.Documents.Create)]
public sealed record UploadDocumentCommand(UpdateDocumentRequest Document, string FileName, long Size, Stream Content) : ICommand<Result<DocumentDto>>;
