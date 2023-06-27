using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Components.ObjectEdit;
using MudBlazor.Extensions.Components.ObjectEdit.Options;
using Nextended.Core.Extensions;
using Coworkee.Application.Configurations;
using Coworkee.Client.Configuration;
using Coworkee.Client.Extensions;
using Coworkee.Client.JsInterop;

namespace Coworkee.Client.Pages.Administration;

public partial class Settings
{
    public ServerConfiguration ServerConfiguration { get; set; }
    // public ClientApplicationConfiguration ClientConfiguration { get; set; }
    private bool _isLoading = true;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
            await _jsRuntime.ObserveMudTabsForStickMerge(".stick-observe");
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        ServerConfiguration = await LoadModel();
        _isLoading = false;
        StateHasChanged();
    }

    private Action<ObjectEditMeta<ClientApplicationConfiguration>> ConfigureClientSettings()
    {
        return meta =>
        {
            meta.Property(c => c.Logging.LogLevel.Default).RenderWithMudAutocomplete<string>(typeof(Microsoft.Extensions.Logging.LogLevel), false);
            meta.Property(c => c.Logging.LogLevel.Microsoft).RenderWithMudAutocomplete<string>(typeof(Microsoft.Extensions.Logging.LogLevel), false);
            meta.Property(c => c.Logging.LogLevel.MicrosoftEntityFrameworkCore).Ignore();
            meta.Property(c => c.ServerConfiguration).Children.Recursive(om => om.Children).Ignore();
        };
    }
    private Action<ObjectEditMeta<ServerConfiguration>> ConfigureServerSettings()
    {
        return meta =>
        {
            meta.Properties(c => c.ClientUrl, c => c.PublicSettings.HostClientInServer).Ignore();
            //meta.Property(c => c.Logging.LogLevel.Default).RenderWithMudAutocomplete<string>(typeof(Microsoft.Extensions.Logging.LogLevel), false);
            //meta.Property(c => c.Logging.LogLevel.Microsoft).RenderWithMudAutocomplete<string>(typeof(Microsoft.Extensions.Logging.LogLevel), false);
            //meta.Property(c => c.Logging.LogLevel.MicrosoftHostingLifetime).RenderWithMudAutocomplete<string>(typeof(Microsoft.Extensions.Logging.LogLevel), false);
            //meta.Property(c => c.Logging.LogLevel.Hangfire).RenderWithMudAutocomplete<string>(typeof(Microsoft.Extensions.Logging.LogLevel), false);
            meta.Property(c => c.ConnectionStrings.DefaultConnection).AsReadOnly().WrapInMudItem(i => i.xs = 12);
            meta.Property(c => c.PublicSettings.UserRegistration.RegistrationDocumentTypes).WrapInMudItem(i => i.xs = 12);
            meta.WrapEachInMudItem(i =>
            {
                i.xl = 6;
                i.xs = 12;
            });
        };
    }

    private async Task OnSubmitServerConfiguration(EditContext arg)
    {
        await _api.System_WriteSystemConfigurationAsync(ServerConfiguration);
        _config.ServerConfiguration = ServerConfiguration.PublicSettings;
        _snackBar.Add(_localizer["saved"], Severity.Success);
    }

    private async Task OnSubmitClientConfiguration(EditContext arg)
    {
        //ClientConfiguration.ServerConfiguration = _config.ServerConfiguration;
        //_config = ClientConfiguration.Clone();

        //await System.IO.File.WriteAllTextAsync("appsettings.json", JsonConvert.SerializeObject(_config, Formatting.Indented));
        _snackBar.Add(_localizer["Saving of Client Settings not possible yet"], Severity.Warning);
    }

    private Task OnCancel()
    {
        _navigationManager.GoBack();
        return Task.CompletedTask;
    }

    private Task<ServerConfiguration> LoadModel() => _api.System_SystemConfigurationAsync();

    private async Task OnRestartClick()
    {
        var res = await _dialogService.ShowConfirmationDialogAsync("Restart server", "Are you sure you want to restart Backend/Server?", icon: Icons.Material.Filled.ConnectedTv);
        if (res)
        {
            await _api.System_RestartServerAsync();
            await _healthCheckService.WaitUntilConnectedAsync("Restart server", "The server is just restarting. Please wait until the connection has been restored");
            _snackBar.Add(_localizer["Server restarted successfully"], Severity.Success);
        }
    }

    private async Task OnRestoreClick()
    {
        var res = await _dialogService.ShowConfirmationDialogAsync("Restore default", "Are you sure you want to restore the default settings? This will overwrite any changes you've ever made here.", icon: Icons.Material.Filled.RestorePage);
        if (res)
        {
            _isLoading = true;
            StateHasChanged();
            var url = _navigationManager.Uri;
            await _api.System_RestoreSystemConfigurationAsync();
            ServerConfiguration = await LoadModel();
            _navigationManager.NavigateToHome();
            _navigationManager.NavigateTo(url);
            StateHasChanged();
        }
    }
}