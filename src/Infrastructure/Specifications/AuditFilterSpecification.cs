using System.Linq;
using CleanArchitectureBase.Infrastructure.Models.Audit;
using CleanArchitectureBase.Application.Specifications.Base;

namespace CleanArchitectureBase.Infrastructure.Specifications
{
    public class AuditFilterSpecification : SpecificationBase<Audit>
    {
        public AuditFilterSpecification(string[] userIds, string searchString, bool searchInOldValues, bool searchInNewValues)
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                Criteria = p => (p.TableName.Contains(searchString) || searchInOldValues && p.OldValues.Contains(searchString) || searchInNewValues && p.NewValues.Contains(searchString)) && (!userIds.Any() || userIds.Contains(p.UserId));
            }
            else
            {
                Criteria = p => (!userIds.Any() || userIds.Contains(p.UserId));
            }
        }
    }
}