namespace CleanArchitectureBase.Client.Infrastructure.Routes
{
    public class DashboardEndpoints
    {
        public static string GetData = $"{BaseEndpoints.Api}/dashboard";
        public static string GetJobsUrl = $"{BaseEndpoints.Api}/dashboard/jobdashboardurl";
    }
}