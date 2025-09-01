using Coworkee.Application.Specifications.Base;
using Coworkee.Domain.Entities.Catalog;

namespace Coworkee.Application.Specifications.Catalog
{
    public class BrandFilterSpecification : SpecificationBase<Brand>
    {
        public BrandFilterSpecification(string searchString)
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                Criteria = p => p.Name.Contains(searchString) || p.Description.Contains(searchString);
            }
            else
            {
                Criteria = p => true;
            }
        }
    }
}
