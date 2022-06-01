using CleanArchitectureBase.Application.Specifications.Base;
using CleanArchitectureBase.Domain.Entities.Misc;

namespace CleanArchitectureBase.Application.Specifications.Misc
{
    public class DocumentFilterSpecification : SpecificationBase<Document>
    {
        public DocumentFilterSpecification(string searchString, string userId, bool userIsAdmin)
        {
            Includes.Add(a => a.DocumentType);
            if (!string.IsNullOrEmpty(searchString))
            {
                Criteria = p => (p.Title.Contains(searchString) || p.Description.Contains(searchString)) && (userIsAdmin || p.IsPublic == true || (p.IsPublic == false && p.CreatedBy == userId));
            }
            else
            {
                Criteria = p => (userIsAdmin || p.IsPublic == true || (p.IsPublic == false && p.CreatedBy == userId));
            }
        }
    }
}