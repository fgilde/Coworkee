
namespace CleanArchitectureBase.Application.Configurations
{
    public class ServerConfiguration : Rootobject
    {
        public ServerConfiguration()
        { }
    }

    // Content here is generated. To generate new open appsettings.json from WebServer project select all and copy content. After wards here in this class use VS->Edit->Paste Special -> Paste JSON as classes
    
    public class Rootobject
    {
        public Logging Logging { get; set; }
        public string AllowedHosts { get; set; }
        public Appconfiguration AppConfiguration { get; set; }
        public Connectionstrings ConnectionStrings { get; set; }
        public Apidocumentation ApiDocumentation { get; set; }
        public Mailconfiguration MailConfiguration { get; set; }
        public Serilog Serilog { get; set; }
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
        public string Secret { get; set; }
    }

    public class Connectionstrings
    {
        public string DefaultConnection { get; set; }
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


}