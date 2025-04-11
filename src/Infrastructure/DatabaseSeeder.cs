using System;
using Coworkee.Infrastructure.Contexts;
using System.Linq;
using Coworkee.Application.Contracts.Services;
using Coworkee.Application.Contracts.Services.Identity;
using Coworkee.Infrastructure.Helpers;
using Coworkee.Infrastructure.Models.Identity;
using Coworkee.Shared.Constants.Application;
using Coworkee.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Nextended.Core.Attributes;
using Nextended.Core.Extensions;

namespace Coworkee.Infrastructure
{
    [RegisterAs(typeof(IDatabaseSeeder), 0, ServiceLifetime = ServiceLifetime.Transient)]
    public class DatabaseSeeder(ApplicationDbContext db, RoleManager<ApplicationRole> roleManager, IUserService userService) : IDatabaseSeeder
    {
        public void Initialize()
        {
            if (ApplicationConstants.IsNswagGeneration)
                return;
            db.Database.EnsureCreated();
            AddRoles();
            AddUsers(new[] { ApplicationConstants.Defaults.Users.System }
                .Concat(ApplicationConstants.Defaults.Users.Administrators)
                .Concat(ApplicationConstants.Defaults.Users.Basic).ToArray());
        }

        private void AddRoles()
        {
            if (db.Roles.Any()) return;
            foreach (var roleTuple in ApplicationConstants.Defaults.Roles.EmptyIfNull())
            {
                if (roleManager.RoleExistsAsync(roleTuple.Name).Result) continue;
                var applicationRole = new ApplicationRole(roleTuple.Name, roleTuple.Name)
                {
                    CreatedBy = ApplicationConstants.Defaults.Users.System.UserName,
                    LastModifiedBy = ApplicationConstants.Defaults.Users.System.UserName,
                    CreatedOn = DateTime.UtcNow,
                    LastModifiedOn = DateTime.UtcNow,
                    Description = roleTuple.Name,
                    IsSelectableByUser = roleTuple.SelectableOnRegistration
                };
                roleManager.CreateAsync(applicationRole).Wait();
                var roleInDb = roleManager.FindByNameAsync(roleTuple.Name).Result;
                foreach (var permission in roleTuple.Permissions)
                    roleManager.AddPermissionClaim(roleInDb, permission).Wait();
            }
        }

        private void AddUsers(CreateUser[] users)
        {
            userService.GetOrAddUserAsync(users).GetAwaiter().GetResult();
        }

    }
}