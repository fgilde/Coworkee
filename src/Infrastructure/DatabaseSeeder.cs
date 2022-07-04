using CleanArchitectureBase.Infrastructure.Contexts;
using System.Linq;
using CleanArchitectureBase.Application.Contracts.Services;
using CleanArchitectureBase.Application.Contracts.Services.Identity;
using CleanArchitectureBase.Infrastructure.Helpers;
using CleanArchitectureBase.Infrastructure.Models.Identity;
using CleanArchitectureBase.Shared.Constants.Application;
using CleanArchitectureBase.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Nextended.Core.Extensions;

namespace CleanArchitectureBase.Infrastructure
{
    public class DatabaseSeeder : IDatabaseSeeder
    {
        private readonly ApplicationDbContext _db;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IUserService _userService;
  

        public DatabaseSeeder(
            ApplicationDbContext db,
            RoleManager<ApplicationRole> roleManager,
            IUserService userService)
        {
            _db = db;
            _roleManager = roleManager;
            _userService = userService;
        }

        public void Initialize()
        {
            AddRoles();
            AddUsers(new[] { ApplicationConstants.Defaults.Users.System }
                .Concat(ApplicationConstants.Defaults.Users.Administrators)
                .Concat(ApplicationConstants.Defaults.Users.Basic).ToArray());
        }

        private void AddRoles()
        {
            if (_db.Roles.Any()) return;
            foreach (var roleTuple in ApplicationConstants.Defaults.Roles.EmptyIfNull())
            {
                if (_roleManager.RoleExistsAsync(roleTuple.Name).Result) continue;
                _roleManager.CreateAsync(new ApplicationRole(roleTuple.Name) { IsSelectableByUser = roleTuple.SelectableOnRegistration }).Wait();
                var roleInDb = _roleManager.FindByNameAsync(roleTuple.Name).Result;
                foreach (var permission in roleTuple.Permissions)
                    _roleManager.AddPermissionClaim(roleInDb, permission).Wait();
            }
        }

        private void AddUsers(CreateUser[] users)
        {
            _userService.GetOrAddUserAsync(users).GetAwaiter().GetResult();
        }

    }
}