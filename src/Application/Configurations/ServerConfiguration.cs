
namespace CleanArchitectureBase.Application.Configurations
{
    public class ServerConfiguration : Rootobject
    {
        public static ServerConfiguration Instance { get; set; }

        public ServerConfiguration()
        { }
    }

    // Content here is generated. To generate new open appsettings.json from WebServer project select all and copy content. After wards here in this class use VS->Edit->Paste Special -> Paste JSON as classes

    public class Rootobject
    {
        public string ClientUrl { get; set; }
        public Connectionstrings ConnectionStrings { get; set; }
        public Publicsettings PublicSettings { get; set; }
        public Logging Logging { get; set; }
        public string AllowedHosts { get; set; }
        public Appconfiguration AppConfiguration { get; set; }
        public Cognitiveservices CognitiveServices { get; set; }
        public Apidocumentation ApiDocumentation { get; set; }
        public Mailconfiguration MailConfiguration { get; set; }
        public Serilog Serilog { get; set; }
        public Azure Azure { get; set; }
        public Rabbitmq RabbitMQ { get; set; }
    }

    public class Connectionstrings
    {
        public string DefaultConnection { get; set; }
    }

    public class Publicsettings
    {
        public string ContactAddress { get; set; }
        public bool HostClientInServer { get; set; }
        public Userregistration UserRegistration { get; set; }
    }

    public class Userregistration
    {
        public bool Enabled { get; set; }
        public bool RequireAddress { get; set; }
        public bool RequiresAdministratorActivation { get; set; }
        public bool EmailConfirmationRequired { get; set; }
        public Usernamerules UsernameRules { get; set; }
        public Passwordrules PasswordRules { get; set; }
        public bool RequireDocuments { get; set; }
        public int RegistrationDocumentsMaxFileSize { get; set; }
        public string[] RegistrationDocumentTypes { get; set; }
    }

    public class Usernamerules
    {
        public int MinLength { get; set; }
        public bool UsernameCanChangedAfterRegistration { get; set; }
        public bool EmailCanChangedAfterRegistration { get; set; }
    }

    public class Passwordrules
    {
        public int MinLength { get; set; }
        public bool CapitalLetterRequired { get; set; }
        public bool LowercaseLetterRequired { get; set; }
        public bool NumberRequired { get; set; }
    }

    public class Logging
    {
        public Loglevel LogLevel { get; set; }
    }

    public class Loglevel
    {
        public string Default { get; set; }
        public string Microsoft { get; set; }
        public string Hangfire { get; set; }
        public string MicrosoftHostingLifetime { get; set; }
    }

    public class Appconfiguration
    {
        public Idhashing IdHashing { get; set; }
        public string Secret { get; set; }
    }

    public class Idhashing
    {
        public bool Enabled { get; set; }
        public int MinLength { get; set; }
        public bool AllowAccessWithNotHashedId { get; set; }
        public string Salt { get; set; }
    }

    public class Cognitiveservices
    {
        public Translation Translation { get; set; }
    }

    public class Translation
    {
        public string Key { get; set; }
        public string TextTranslationEndpoint { get; set; }
        public string DocumentTranslationEndpoint { get; set; }
        public string Region { get; set; }
    }

    public class Apidocumentation
    {
        public bool RequireLogin { get; set; }
        public bool RequirePermission { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Contact Contact { get; set; }
        public License License { get; set; }
    }

    public class Contact
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Url { get; set; }
    }

    public class License
    {
        public string Name { get; set; }
        public string spdx_id { get; set; }
        public string Url { get; set; }
    }

    public class Mailconfiguration
    {
        public string SendGridApiKey { get; set; }
        public string From { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
    }

    public class Serilog
    {
        public Minimumlevel MinimumLevel { get; set; }
        public Writeto[] WriteTo { get; set; }
        public string[] Enrich { get; set; }
        public Properties Properties { get; set; }
    }

    public class Minimumlevel
    {
        public string Default { get; set; }
        public Override Override { get; set; }
    }

    public class Override
    {
        public string Microsoft { get; set; }
        public string MicrosoftHostingLifetime { get; set; }
        public string System { get; set; }
        public string Hangfire { get; set; }
    }

    public class Properties
    {
        public string Application { get; set; }
    }

    public class Writeto
    {
        public string Name { get; set; }
        public Args Args { get; set; }
    }

    public class Args
    {
        public string outputTemplate { get; set; }
        public string path { get; set; }
        public string rollingInterval { get; set; }
    }

    public class Azure
    {
        public Signalr SignalR { get; set; }
    }

    public class Signalr
    {
        public bool Enabled { get; set; }
        public string StickyServerMode { get; set; }
        public string ConnectionString { get; set; }
    }

    public class Rabbitmq
    {
        public bool Enabled { get; set; }
        public string HostName { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }

}