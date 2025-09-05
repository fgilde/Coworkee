using FluentValidation;
using Microsoft.Extensions.Localization;
using lib.Coworkee.Application.Common.Models.Identity;

namespace Coworkee.Application.Validators.Requests.Identity
{
    public class AddressValidator : AbstractValidator<AddressDto>
    {
        public AddressValidator()
        {
            RuleFor(address => address.Street).NotEmpty();
            RuleFor(address => address.City).NotEmpty();
            RuleFor(address => address.HouseNumber).NotEmpty();
            RuleFor(address => address.PostalCode).NotEmpty();
        }
    }
}