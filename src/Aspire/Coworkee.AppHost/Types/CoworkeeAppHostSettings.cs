namespace Coworkee.AppHost.Types;

public class CoworkeeAppHostSettings : IAppHostServiceSettings
{
    public DatabaseToUse DatabaseToUse { get; set; }
    public bool AddKeycloak { get; set; }
    public bool AddAzureStorage { get; set; }
    public bool AddStirling { get; set; }
    public bool AddGrafana { get; set; }
    public bool AddOllama { get; set; }
}