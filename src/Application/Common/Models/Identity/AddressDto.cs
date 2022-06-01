namespace CleanArchitectureBase.Application.Common.Models.Identity;

public class AddressDto : DtoBase<int>
{
    public string Name { get; set; }
    public string City { get; set; }
    public string Street { get; set; }
    public string Country { get; set; }
    public string PostalCode { get; set; }
    public string HouseNumber { get; set; }
}