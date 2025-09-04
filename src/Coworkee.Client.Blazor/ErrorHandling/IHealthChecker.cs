using System.Threading.Tasks;

namespace Coworkee.Client.ErrorHandling;

public interface IHealthChecker
{
    Task StartCheckHealthAsync();
    Task WaitUntilConnectedAsync(string title = "", string message = "");
}