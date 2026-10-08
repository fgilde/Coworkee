using Coworkee.Application.Messaging;
using Coworkee.Core.Results;
using MyApp.Contracts.Documents;

namespace MyApp.Documents.Features.DocumentTypes.Commands.AddEdit;

[AiTool("Creates a document type, or changes it when Id is given.")]
public sealed record AddEditDocumentTypeCommand(Guid? Id, AddEditDocumentTypeRequest Type) : ICommand<Result<DocumentTypeDto>>;
