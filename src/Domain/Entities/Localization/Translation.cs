using CleanArchitectureBase.Domain.Contracts;

namespace CleanArchitectureBase.Domain.Entities.Localization
{
    public class Translation: AuditableEntity<int>
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string CultureCode { get; set; }
    }
}