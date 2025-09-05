namespace Coworkee.Application.Common.Models.Identity;

public class ExternalLoginOptions
{
    public ExternalLoginOptions(bool registerIfNotExists, string scheme, string token = null)
    {
        RegisterIfNotExists = registerIfNotExists;
        Scheme = scheme;
        Token = token;
    }

    public bool RegisterIfNotExists { get; set; }
    public string Scheme { get; set; }
    public string Token { get; set; }
}