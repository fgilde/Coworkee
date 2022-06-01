using CleanArchitectureBase.Domain.Contracts;

namespace CleanArchitectureBase.Domain.Entities.Identity;

public class Address : AuditableEntity<int>
{
    public string Name { get; set; }
    public string City { get; set; }
    public string Street { get; set; }
    public string Country { get; set; }
    public string PostalCode { get; set; }
    public string HouseNumber { get; set; }
}