using CleanArchitectureBase.Infrastructure.Contexts;
using System.Linq;
using CleanArchitectureBase.Application.Contracts.Services;
using CleanArchitectureBase.Application.Contracts.Services.Identity;
using CleanArchitectureBase.Shared.Constants.Application;
using CleanArchitectureBase.Shared.Models;

namespace CleanArchitectureBase.Infrastructure
{
    public class DatabaseSeeder : IDatabaseSeeder
    {
        private readonly IUserService _userService;
  

        public DatabaseSeeder(
            ApplicationDbContext db,
            IUserService userService)
        {
            _userService = userService;
        }

        public void Initialize()
        {
            var usersToCreate = new[] {ApplicationConstants.Defaults.Users.System}
                .Concat(ApplicationConstants.Defaults.Users.Administrators)
                .Concat(ApplicationConstants.Defaults.Users.Basic).ToArray();
            AddUsers(usersToCreate);
        }


        private void AddUsers(CreateUser[] users)
        {
            _userService.GetOrAddUserAsync(users).GetAwaiter().GetResult();
        }

    }
}