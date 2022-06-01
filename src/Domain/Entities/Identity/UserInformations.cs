using System;
using System.Collections.Generic;
using CleanArchitectureBase.Domain.Contracts;
using CleanArchitectureBase.Domain.Enums;

namespace CleanArchitectureBase.Domain.Entities.Identity;

public class UserInformations: AuditableEntity<int>
{
    public virtual ICollection<Address> Addresses { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public bool IsOnline { get; set; }
}