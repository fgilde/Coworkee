using Coworkee.Application.Messaging;
using Coworkee.Core.Results;
using Coworkee.Files;
using MyApp.Contracts.Documents;
using MyApp.Documents.Features.Documents.Commands.Upload;

namespace MyApp.Documents.Features.Documents.Commands.Import;

/// <summary>Reads the file through the Files module as the current user, so its folder permissions apply, and uploads a copy as a document.</summary>
internal sealed class ImportDocumentHandler(IDispatcher dispatcher) : IHandler<ImportDocumentCommand, Result<DocumentDto>>
{
    public async Task<Result<DocumentDto>> HandleAsync(ImportDocumentCommand command, CancellationToken cancellationToken)
    {
        var fileId = command.Request.FileId;
        var file = await dispatcher.SendAsync(new GetFile(fileId), cancellationToken);
        if (!file.IsSuccess)
        {
            return file.Error!;
        }

        var opened = await dispatcher.SendAsync(new OpenFile(fileId), cancellationToken);
        if (!opened.IsSuccess)
        {
            return opened.Error!;
        }

        await using var content = opened.Value.Content;
        return await dispatcher.SendAsync(new UploadDocumentCommand(command.Request.Document, file.Value.Name, file.Value.Size, content), cancellationToken);
    }
}
