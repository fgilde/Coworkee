using Microsoft.Extensions.Configuration;

namespace CleanArchitectureBase.Client.Configuration
{
    public class ClientApplicationConfiguration
    {
        public string BackendOrigin { get; set; }
        public Logging Logging { get; set; }

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