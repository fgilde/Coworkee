namespace CleanArchitectureBase.Server;

public class ServerUtils
{
    public static bool ClientRunsOnServer
    {
        get
        {
            #if HostClient
                return true;
            #else
                return false;
            #endif
        }
    }
}