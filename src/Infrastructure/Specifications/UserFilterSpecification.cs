using CleanArchitectureBase.Infrastructure.Models.Identity;
using CleanArchitectureBase.Application.Specifications.Base;

namespace CleanArchitectureBase.Infrastructure.Specifications
{
    public class UserFilterSpecification : SpecificationBase<ApplicationUser>
    {
        public UserFilterSpecification(string searchString)
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                Criteria = p => p.FirstName.Contains(searchString) || p.LastName.Contains(searchString) || p.Email.Contains(searchString) || p.PhoneNumber.Contains(searchString) || p.UserName.Contains(searchString);
            }
            else
            {
                Criteria = p => true;
            }
        }
    }
}