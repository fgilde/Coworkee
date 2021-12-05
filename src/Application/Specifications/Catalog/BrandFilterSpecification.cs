using CleanArchitectureBase.Application.Specifications.Base;
using CleanArchitectureBase.Domain.Entities.Catalog;

namespace CleanArchitectureBase.Application.Specifications.Catalog
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
