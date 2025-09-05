
using lib.Coworkee.Shared.Constants.Application;
using Microsoft.IdentityModel.Tokens;

namespace lib.Coworkee.Application.Configurations
{
    public partial class ServerConfiguration
    {
        public static ServerConfiguration Instance { get; set; }
    }
    

    public partial class PublicSettings
    {
        public bool KeycloakEnabled 
            => Endpoints.TryGetValue(ApplicationConstants.ServiceNames.Keycloak, out string keycloakUrl) 
               && !string.IsNullOrEmpty(keycloakUrl) 
               && this.LoginSettings?.LoginMode != LoginMode.Internal;
        public System.Collections.Generic.Dictionary<string, string> Endpoints { get; set; }

    }

    public partial class LoginSettings
    {
        public LoginMode LoginMode { get; set; }

    }
    

    public enum LoginMode
    {
        Both,
        External,
        Internal
    }
}