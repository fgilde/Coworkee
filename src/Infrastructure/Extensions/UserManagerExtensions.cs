using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CleanArchitectureBase.Infrastructure.Models.Identity;

namespace CleanArchitectureBase.Infrastructure.Extensions;

public static class UserManagerExtensions
{
    public static IQueryable<ApplicationUser> LoadedUsers(this UserManager<ApplicationUser> userManager)
        => userManager.Users.Include(u => u.UserInfo).Include(u => u.UserInfo.Addresses);
    public static Task<ApplicationUser> FindByAsync(this UserManager<ApplicationUser> userManager, Expression<Func<ApplicationUser, bool>> expression)
        => userManager.LoadedUsers().FirstOrDefaultAsync(expression);
    public static Task<ApplicationUser> FindByIdFullyLoadedAsync(this UserManager<ApplicationUser> userManager, string id)
        => userManager.FindByAsync(u => u.Id == id);
    public static Task<ApplicationUser> FindByEmailFullyLoadedAsync(this UserManager<ApplicationUser> userManager, string email)
        => userManager.FindByAsync(u => u.Email == email);
    public static Task<ApplicationUser> FindByNameFullyLoadedAsync(this UserManager<ApplicationUser> userManager, string name)
        => userManager.FindByAsync(u => u.UserName == name);
}