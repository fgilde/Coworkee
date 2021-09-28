using System.ComponentModel.DataAnnotations;
using CleanArchitectureBase.Application.Interfaces.Repositories;
using CleanArchitectureBase.Domain.Entities.Catalog;
using CleanArchitectureBase.Shared.Wrapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using CleanArchitectureBase.Shared.Constants.Application;
using Nextended.Core.Extensions;

namespace CleanArchitectureBase.Application.Features.Brands.Commands.AddEdit
{
    public partial class AddEditBrandCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public decimal Tax { get; set; }
    }

    internal class AddEditBrandCommandHandler : IRequestHandler<AddEditBrandCommand, Result<int>>
    {
        private readonly IStringLocalizer<AddEditBrandCommandHandler> _localizer;
        private readonly IUnitOfWork<int> _unitOfWork;

        public AddEditBrandCommandHandler(IUnitOfWork<int> unitOfWork, IStringLocalizer<AddEditBrandCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<Result<int>> Handle(AddEditBrandCommand command, CancellationToken cancellationToken)
        {
            if (command.Id == 0)
            {
                var brand = command.MapTo<Brand>();
                await _unitOfWork.Repository<Brand>().AddAsync(brand);
                await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllBrandsCacheKey);
                return await Result<int>.SuccessAsync(brand.Id, _localizer["Brand Saved"]);
            }
            else
            {
                var brand = await _unitOfWork.Repository<Brand>().GetByIdAsync(command.Id);
                if (brand != null)
                {
                    brand.Name = command.Name ?? brand.Name;
                    brand.Tax = (command.Tax == 0) ? brand.Tax : command.Tax;
                    brand.Description = command.Description ?? brand.Description;
                    await _unitOfWork.Repository<Brand>().UpdateAsync(brand);
                    await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllBrandsCacheKey);
                    return await Result<int>.SuccessAsync(brand.Id, _localizer["Brand Updated"]);
                }

                return await Result<int>.FailAsync(_localizer["Brand Not Found!"]);
            }
        }
    }
}