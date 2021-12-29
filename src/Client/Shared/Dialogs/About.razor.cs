using System.Collections.Generic;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Shared.Constants.Application;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CleanArchitectureBase.Client.Shared.Dialogs
{
    public partial class About
    {
        [Parameter] public VersionInfoModel ClientInfo { get; set; }
        [Parameter] public VersionInfoModel ServerInfo { get; set; }
        [Parameter] public IList<string> ApiVersions { get; set; }

        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }


        public void Close()
        {
            MudDialog.Close();
        }

        protected override async Task OnInitializedAsync()
        {
            ClientInfo ??= new VersionInfoModel {ApplicationName = ApplicationConstants.ApplicationClientName};
            ServerInfo ??= await _api.System_VersionAsync();
            ApiVersions ??= await _api.System_AvailableApiVersionsAsync();
        }
    }
}