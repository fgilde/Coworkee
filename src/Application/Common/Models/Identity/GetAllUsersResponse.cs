using System.Collections.Generic;

namespace CleanArchitectureBase.Application.Common.Models.Identity
{
    public class GetAllUsersResponse
    {
        public IEnumerable<UserResponse> Users { get; set; }
    }
}