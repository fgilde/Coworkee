using Microsoft.Extensions.Configuration;
using lib.Coworkee.Application.Configurations;

namespace lib.Coworkee.Client.Configuration
{
    public class ClientApplicationConfiguration
    {
        public string BackendOrigin { get; set; }
        public string JsMainNamespace { get; set; }
        public bool AllowAnonymousPageAccess { get; set; }
        public int BackendHealthCheckIntervalInSeconds { get; set; }
        public Logging Logging { get; set; }
        public PublicSettings ServerConfiguration { get; set; }

        public static ClientApplicationConfiguration Create(IConfiguration configuration)
        {
            var result = new ClientApplicationConfiguration();
            configuration.Bind(result);
            return result;
        }
    }

    public class Logging
    {
        public Loglevel LogLevel { get; set; }
    }

    public class Loglevel
    {
        public string Default { get; set; }
        public string Microsoft { get; set; }
        public string MicrosoftEntityFrameworkCore { get; set; }
    }
}