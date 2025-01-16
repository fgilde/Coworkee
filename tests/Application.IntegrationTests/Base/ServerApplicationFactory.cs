using System;
using System.Threading.Tasks;
using Coworkee.Infrastructure.Contexts;
using Coworkee.Shared.Constants.Application;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Xunit;

namespace Coworkee.Application.IntegrationTests.Base;

public class ServerApplicationFactory : WebApplicationFactory<Coworkee.Server.Program>, IAsyncLifetime
{
    internal int Port = Random.Shared.Next(10000, 50000);
    internal string DbPassword = "DB4TestsPassword123";
    internal string DbName = "Coworkee_Test_DB";
    private Task _dbContainerTask = null;

    // each class with tests has its own container
    private IContainer _dbContainer => new ContainerBuilder().WithImage("mcr.microsoft.com/mssql/server:2019-latest")
        .WithEnvironment("ACCEPT_EULA", "Y")
        .WithEnvironment("SA_PASSWORD", DbPassword)
        .WithPortBinding(Port, 1433)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(1433))
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        if (_dbContainerTask == null)
        {
            InitializeAsync().Wait();
        }
        base.ConfigureWebHost(builder);
        builder.UseEnvironment(ApplicationConstants.Environment.Testing);        
        builder.ConfigureTestServices(services =>
        {
            // Database for Tests
            string connectionStr = $"Server=localhost,{Port};User Id=sa;Password={DbPassword};Database={DbName};Encrypt=False;TrustServerCertificate=True;";
            services.RemoveAll(typeof(ApplicationDbContext));            
            services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(
                    connectionStr,
                    builder =>
                    {
                        builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(2), null);
                    })); 
        });
    }


    public Task InitializeAsync()
    {
        return _dbContainerTask = _dbContainer.StartAsync();
    }

    public new Task DisposeAsync()
    {
        _dbContainerTask = null;
        return _dbContainer.StopAsync();
    }
}
