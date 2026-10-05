using Coworkee.Application.Messaging;
using Coworkee.Core.Results;
using MyApp.Contracts.Documents;

namespace MyApp.Documents.Features.DocumentTypes.Commands.AddEdit;

public sealed record AddEditDocumentTypeCommand(Guid? Id, AddEditDocumentTypeRequest Type) : ICommand<Result<DocumentTypeDto>>;
