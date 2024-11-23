using System;
using System.Linq;
using Coworkee.Shared.Constants.Application;
using DefaultUsers = Coworkee.Shared.Constants.Application.ApplicationConstants.Defaults.Users;

namespace Coworkee.Shared.Models;

public class CreateUser
{
    public CreateUser(string userName, string firstName, string lastName, string email, string password, bool isSuperUser, string roleToAdd)
    {
        UserName = userName;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Password = password;
        IsSuperUser = isSuperUser;
        RoleToAdd = roleToAdd;
    }

    public CreateUser()
    {}

    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public bool IsSuperUser { get; set; }
    public string RoleToAdd { get; set; }

    public bool IsSystemUser() => DefaultUsers.System.UserName == UserName || DefaultUsers.System.Email == Email;
}