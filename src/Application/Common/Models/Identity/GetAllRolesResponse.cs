using System.Collections.Generic;

namespace CleanArchitectureBase.Application.Common.Models.Identity
{
    public class GetAllRolesResponse
    {
        public IEnumerable<RoleResponse> Roles { get; set; }
    }
}