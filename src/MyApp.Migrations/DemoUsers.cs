namespace MyApp.Migrations;

/// <summary>
/// Development users: seeded into the app and, with Keycloak, into its realm with the same password (see LOGIN.md).
/// The passwords are for development only; change them before you deploy anywhere.
/// </summary>
public static class DemoUsers
{
    public static readonly DemoUser Administrator = new("info@coworkee.de", "Administrator", null, "123Pa$$word!", IsAdmin: true);

    public static readonly DemoUser Florian = new("fgilde@gmail.com", "Florian", "Gilde", "123Pa$$word!", IsAdmin: true);

    public static readonly DemoUser John = new("john@coworkee.de", "John", "Doe", "123Pa$$word!");

    public static readonly DemoUser[] All = [Administrator, Florian, John];
}

public sealed record DemoUser(string Email, string FirstName, string? LastName, string Password, bool IsAdmin = false, string[]? Roles = null);
