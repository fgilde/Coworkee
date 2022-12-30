namespace Coworkee.Application.Common.Models.Identity;

public class RoleDto : DtoBase<string>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsSelectableByUser { get; set; }
}