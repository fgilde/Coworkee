using System;
using System.Collections.Generic;

namespace Coworkee.Application.Common.Models.Identity;

public class UserInformationsDto : DtoBase<int>
{
    public ICollection<AddressDto> Addresses { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public string Language { get; set; }
    public bool IsOnline { get; set; }
}