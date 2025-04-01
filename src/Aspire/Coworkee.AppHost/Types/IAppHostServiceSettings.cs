namespace Coworkee.AppHost.Types;

public interface IAppHostServiceSettings
{
    DatabaseToUse DatabaseToUse { get; }
    bool AddKeycloak { get; }
    bool AddAzureStorage { get; }
    bool AddStirling { get; }
    bool AddGrafana { get; }
    bool AddOllama { get; }
}