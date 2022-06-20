using System;
using System.Globalization;
using System.Threading.Tasks;
using CleanArchitectureBase.Client.Extensions;
using CleanArchitectureBase.Client.Localization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;

namespace CleanArchitectureBase.Client;

public partial class App : IDisposable
{
    private HubConnection hubConnection;
    private AuthenticationState authenticationState;
    protected override async Task OnInitializedAsync()
    {
        authenticationState = await _stateProvider.GetAuthenticationStateAsync();
        await ApiResources.UpdateEntries(_api, CultureInfo.DefaultThreadCurrentCulture, false);
        _interceptor.RegisterEvent();
        hubConnection = await hubConnection.EnsureStartedAsync(_config.BackendOrigin);
    }
    
    public void Dispose()
    {
        _snackBar?.Dispose();
        _httpClient?.Dispose();
        _interceptor.DisposeEvent();
        _ = hubConnection.DisposeAsync();
    }

    private bool IsLoggedIn()
    {
        var result = authenticationState?.User.Identity?.IsAuthenticated == true;
        return result || _stateProvider.IsAuthenticatedBeforeEvent;
    }
}