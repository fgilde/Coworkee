using lib.Coworkee.Application.Specifications.Base;
using lib.Coworkee.Domain.Entities.Misc;

namespace lib.Coworkee.Application.Specifications.Misc
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