using System;
using System.Collections.Generic;
using Coworkee.Shared.Models;
using Nextended.Core.Extensions;

namespace Coworkee.Application.Common.Models.Identity
{
    public class UserResponse : DtoBase<string>
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; } = true;
        public bool EmailConfirmed { get; set; }
        public string PhoneNumber { get; set; }
        public string ProfilePictureDataUrl { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public bool IsOnline { get; set; }
        public UserInformationsDto UserInfo { get; set; }
        public DateTime CreatedOn { get; set; }
        public List<UserRoleModel> Roles { get; set; } = new();
        public bool IsSystemUser() => this.MapTo<CreateUser>().IsSystemUser();
    }
}