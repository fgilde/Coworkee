using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Interfaces.Repositories;
using CleanArchitectureBase.Domain.Entities.Misc;
using CleanArchitectureBase.Shared.Constants.Application;
using CleanArchitectureBase.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Nextended.Core.Extensions;

namespace CleanArchitectureBase.Application.Features.DocumentTypes.Commands.AddEdit
{
    public class AddEditDocumentTypeCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
    }

    internal class AddEditDocumentTypeCommandHandler : IRequestHandler<AddEditDocumentTypeCommand, Result<int>>
    {
        private readonly IStringLocalizer<AddEditDocumentTypeCommandHandler> _localizer;
        private readonly IUnitOfWork<int> _unitOfWork;

        public AddEditDocumentTypeCommandHandler(IUnitOfWork<int> unitOfWork, IStringLocalizer<AddEditDocumentTypeCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<Result<int>> Handle(AddEditDocumentTypeCommand command, CancellationToken cancellationToken)
        {
            if (await _unitOfWork.Repository<DocumentType>().Entities.Where(p => p.Id != command.Id)
                .AnyAsync(p => p.Name == command.Name, cancellationToken))
            {
                return await Result<int>.FailAsync(_localizer["Document type with this name already exists."]);
            }

            if (command.Id == 0)
            {
                var documentType = command.MapTo<DocumentType>();
                await _unitOfWork.Repository<DocumentType>().AddAsync(documentType);
                await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllDocumentTypesCacheKey);
                return await Result<int>.SuccessAsync(documentType.Id, _localizer["Document Type Saved"]);
            }
            else
            {
                var documentType = await _unitOfWork.Repository<DocumentType>().GetByIdAsync(command.Id);
                if (documentType != null)
                {
                    documentType.Name = command.Name ?? documentType.Name;
                    documentType.Description = command.Description ?? documentType.Description;
                    await _unitOfWork.Repository<DocumentType>().UpdateAsync(documentType);
                    await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllDocumentTypesCacheKey);
                    return await Result<int>.SuccessAsync(documentType.Id, _localizer["Document Type Updated"]);
                }
                else
                {
                    return await Result<int>.FailAsync(_localizer["Document Type Not Found!"]);
                }
            }
        }
    }
}