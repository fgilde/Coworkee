using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using MudBlazor.Extensions.Components.ObjectEdit;
using MudBlazor.Extensions.Components.ObjectEdit.Options;
using Nextended.Core.Extensions;
using CleanArchitectureBase.Application.Configurations;
using CleanArchitectureBase.Client.Configuration;
using CleanArchitectureBase.Client.Extensions;
using CleanArchitectureBase.Client.JsInterop;

namespace CleanArchitectureBase.Client.Pages.Administration;

public partial class Settings
{
    public ServerConfiguration ServerConfiguration { get; set; }
   // public ClientApplicationConfiguration ClientConfiguration { get; set; }
    private bool _isLoading = true;

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            _jsRuntime.ObserveMudTabsForStickMerge(".stick-observe");

        return base.OnAfterRenderAsync(firstRender);
    }
    
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        //ClientConfiguration = _config.Clone();
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
    private Action<ObjectEditMeta<ServerConfiguration>> Configure()
    {
        return meta =>
        {
            meta.Property(c => c.Logging.LogLevel.Default).RenderWithMudAutocomplete<string>(typeof(Microsoft.Extensions.Logging.LogLevel), false);
            meta.Property(c => c.Logging.LogLevel.Microsoft).RenderWithMudAutocomplete<string>(typeof(Microsoft.Extensions.Logging.LogLevel), false);
            meta.Property(c => c.Logging.LogLevel.MicrosoftHostingLifetime).RenderWithMudAutocomplete<string>(typeof(Microsoft.Extensions.Logging.LogLevel), false);
            meta.Property(c => c.Logging.LogLevel.Hangfire).RenderWithMudAutocomplete<string>(typeof(Microsoft.Extensions.Logging.LogLevel), false);
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

}