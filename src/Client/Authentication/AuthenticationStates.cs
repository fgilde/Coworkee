using System;
using System.Security.Claims;
using CleanArchitectureBase.Client.Extensions;
using Microsoft.AspNetCore.Components.Authorization;

namespace CleanArchitectureBase.Client.Authentication;

public static class AuthenticationStates
{
    public static bool IsGuest(this AuthenticationState state) => state.User.IsGuest();
    public static AuthenticationState Guest => new(new ClaimsPrincipal(
        new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, nameof(Guest)), 
            new Claim(ClaimTypes.Surname, nameof(Guest)), 
            new Claim(ClaimTypes.NameIdentifier, Guid.Empty.ToString())
        }, nameof(Guest))));

    public static AuthenticationState None => new(new ClaimsPrincipal(new ClaimsIdentity()));
}