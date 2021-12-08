using CleanArchitectureBase.Domain.Contracts;

namespace CleanArchitectureBase.Domain.Entities.Localization;

public class Language: AuditableEntity<int>
{
    public bool IsRTL { get; set; }
    public bool IsActive { get; set; }
    public string CultureCode { get; set; }
    public string DisplayName { get; set; }
}