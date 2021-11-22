using System.Collections.Generic;

namespace CleanArchitectureBase.Application.Common.Models.Identity
{
    public class PermissionResponse
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public List<RoleClaimResponse> RoleClaims { get; set; }
    }
}