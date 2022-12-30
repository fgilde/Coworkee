using Coworkee.Domain.Contracts;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Coworkee.Application.Common.Models.Chat;
using Coworkee.Application.Contracts.Chat;
using Coworkee.Domain.Entities.Identity;
using Coworkee.Shared.Constants.Application;
using Coworkee.Shared.Models;
using Nextended.Core.Extensions;

namespace Coworkee.Infrastructure.Models.Identity
{
    public class ApplicationUser : IdentityUser<string>, IChatUser, IAuditableEntity<string>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CreatedBy { get; set; }

        [Column(TypeName = "text")]
        public string ProfilePictureDataUrl { get; set; }
        public DateTime CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
        public bool IsActive { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public virtual ICollection<ChatHistory<ApplicationUser>> ChatHistoryFromUsers { get; set; }
        public virtual ICollection<ChatHistory<ApplicationUser>> ChatHistoryToUsers { get; set; }
        public bool IsSystemUser() => this.MapTo<CreateUser>().IsSystemUser();
        public virtual UserInformations UserInfo { get; set; }
        public bool IsOnline => UserInfo is { IsOnline: true } && UserInfo?.LastLoginDate > DateTime.UtcNow.AddDays(-ApplicationConstants.Session.RefreshTokenExpiryInDays);


        public ApplicationUser()
        {
            ChatHistoryFromUsers = new HashSet<ChatHistory<ApplicationUser>>();
            ChatHistoryToUsers = new HashSet<ChatHistory<ApplicationUser>>();
        }
    }
}