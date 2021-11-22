using System.Collections.Generic;
using CleanArchitectureBase.Application.Common.Models.Identity;

namespace CleanArchitectureBase.Application.Requests.Identity
{
    public class UpdateUserRolesRequest
    {
        public string UserId { get; set; }
        public IList<UserRoleModel> UserRoles { get; set; }
    }
}