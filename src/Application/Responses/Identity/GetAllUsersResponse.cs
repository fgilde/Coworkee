using System.Collections.Generic;

namespace CleanArchitectureBase.Application.Responses.Identity
{
    public class GetAllUsersResponse
    {
        public IEnumerable<UserResponse> Users { get; set; }
    }
}