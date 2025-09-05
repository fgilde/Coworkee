using lib.Coworkee.Domain.Contracts;

namespace Coworkee.Domain.Entities.Catalog
{
    // Brand
    public class Brand : AuditableEntity<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Tax { get; set; }
    }
}