using System.Collections.Generic;
using lib.Coworkee.Application.Common.Models.Identity;

namespace lib.Coworkee.Application.Requests.Identity
{
    public class UpdateUserRolesRequest
    {
        public string UserId { get; set; }
        public IList<UserRoleModel> UserRoles { get; set; }
    }
}