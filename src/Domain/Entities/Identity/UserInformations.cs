using System;
using System.Collections.Generic;
using lib.Coworkee.Domain.Contracts;

namespace Coworkee.Domain.Entities.Identity;

public class UserInformations : AuditableEntity<int>
{
    public virtual ICollection<Address> Addresses { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public string Language { get; set; }
    public bool IsOnline { get; set; }
}