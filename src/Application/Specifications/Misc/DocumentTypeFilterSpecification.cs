using CleanArchitectureBase.Application.Specifications.Base;
using CleanArchitectureBase.Domain.Entities.Misc;

namespace CleanArchitectureBase.Application.Specifications.Misc
{
    public class DocumentTypeFilterSpecification : SpecificationBase<DocumentType>
    {
        public DocumentTypeFilterSpecification(string searchString)
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