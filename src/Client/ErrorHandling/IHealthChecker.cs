using System.Threading.Tasks;

namespace CleanArchitectureBase.Client.ErrorHandling;

public interface IHealthChecker
{
    Task StartCheckHealthAsync();
    Task WaitUntilConnectedAsync(string title = "", string message = "");
}