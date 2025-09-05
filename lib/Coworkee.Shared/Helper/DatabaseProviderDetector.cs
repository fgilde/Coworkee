using System;

namespace Coworkee.Shared.Helper;

public static class DatabaseProviderDetector
{
    public static bool IsSqlConnectionString(string connectionString) => DetectProvider(connectionString) == DatabaseProvider.SqlServer;
    public static bool IsPostgresConnectionString(string connectionString) => DetectProvider(connectionString) == DatabaseProvider.Postgres;

    public static DatabaseProvider DetectProvider(string connectionString)
    {
        // TODO: Implement better detection and for other providers as well
        if (connectionString.Contains("Host=", StringComparison.OrdinalIgnoreCase))
            return DatabaseProvider.Postgres;
        if (connectionString.Contains("Server=", StringComparison.OrdinalIgnoreCase) ||
            connectionString.Contains("Data Source=", StringComparison.OrdinalIgnoreCase))
            return DatabaseProvider.SqlServer;

        return DatabaseProvider.Unknown;
    }
}

public enum DatabaseProvider
{
    Postgres,
    SqlServer,
    Unknown
}
