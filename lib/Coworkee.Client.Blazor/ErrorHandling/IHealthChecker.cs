using System.Threading.Tasks;

namespace lib.Coworkee.Client.ErrorHandling;

public interface IHealthChecker
{
    Task StartCheckHealthAsync();
    Task WaitUntilConnectedAsync(string title = "", string message = "");
}