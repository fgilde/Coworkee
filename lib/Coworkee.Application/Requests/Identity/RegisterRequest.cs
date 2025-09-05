using System.Collections.Generic;
using lib.Coworkee.Application.Common.Models.Identity;

namespace lib.Coworkee.Application.Requests.Identity
{
    public class RegisterRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; } = false;
        public bool EmailConfirmed { get; set; } = false;
        public IList<UploadRequest> Documents { get; set; }
        public UserInformationsDto UserInfo { get; set; } = new();
        public List<string> InitialRoleNames { get; set; } = new();
    }
}