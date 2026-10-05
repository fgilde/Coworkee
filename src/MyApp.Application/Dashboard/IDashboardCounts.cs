namespace MyApp.Application.Dashboard;

public interface IDashboardCounts
{
    Task<(int Documents, int DocumentTypes)> DocumentsAsync(CancellationToken cancellationToken);
}
