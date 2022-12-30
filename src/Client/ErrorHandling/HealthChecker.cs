using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MudBlazor;
using MudBlazor.Extensions;
using Nextended.Core;
using Nextended.Core.Extensions;
using Coworkee.Client.Configuration;

namespace Coworkee.Client.ErrorHandling;

public class HealthChecker : IHealthChecker
{
    private readonly ClientApplicationConfiguration _clientConfig;
    private readonly IDialogService _dialogService;
    private readonly HttpClient _client;
    private readonly string _healthCheckUrl;
    IDialogReference dlg;
    private CancellationTokenSource _cts;
    private Task _checkTask;
    private string defaultTitle = "Not connected";
    private string defaultMessage = "A connection to the server cannot be established. Your network connection may be down or the server is unreachable. Please wait until the connection has been restored";

    public HealthChecker(ClientApplicationConfiguration clientConfig, IDialogService dialogService, IHttpClientFactory clientFactory)
    {
        _healthCheckUrl = $"{clientConfig.BackendOrigin.EnsureEndsWith("/")}health";
        _client = clientFactory.CreateClient("HealthCheck");
        _clientConfig = clientConfig;
        _dialogService = dialogService;
        IsEnabled = clientConfig.BackendHealthCheckIntervalInSeconds > 0;
    }

    public bool IsEnabled
    {
        get => _checkTask is {IsCanceled: false, IsCompleted: false};
        set
        {
            if (!value && _checkTask != null && _cts != null)
            {
                _cts.Cancel();
                _checkTask = null;
                _cts = null;
            }
            if (value && _checkTask == null)
            {
                _ = StartCheckHealthAsync();
            }
        }
    }

    public async Task WaitUntilConnectedAsync(string title = "", string message = "")
    {
        dlg = await _dialogService.ShowInformationAsync(!string.IsNullOrEmpty(title) ? title : defaultTitle, !string.IsNullOrEmpty(message) ? message : defaultMessage, Icons.Material.Filled.ConnectedTv, false, true);
        IsEnabled = true;
        await Waiter.WaitForTrueAsync(() => dlg == null);
    }
    
    public Task StartCheckHealthAsync()
    {
        if (_checkTask != null)
            return _checkTask;
        _cts = new CancellationTokenSource();
        return _checkTask = Task.Run(async () =>
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(_clientConfig.BackendHealthCheckIntervalInSeconds > 0 ? _clientConfig.BackendHealthCheckIntervalInSeconds : 1));
                await _client.GetAsync(_healthCheckUrl).ContinueWith(async task =>
                {
                    bool failed = !task.IsCompletedSuccessfully || !(await task).IsSuccessStatusCode;
                    switch (failed)
                    {
                        case true when dlg == null:
                            dlg = await _dialogService.ShowInformationAsync(defaultTitle, defaultMessage, Icons.Material.Filled.ConnectedTv, false, true);
                            break;
                        case false when dlg != null:
                            dlg.Close();
                            dlg = null;
                            break;
                    }
                });
            }
        }, _cts.Token);
    }
}
