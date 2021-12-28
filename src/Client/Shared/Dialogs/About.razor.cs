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
        private VersionInfoModel serverInfo;
        private VersionInfoModel clientInfo;
        private IList<string> apiVersions;

        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }


        public void Close()
        {
            MudDialog.Close();
        }

        protected override async Task OnInitializedAsync()
        {
            clientInfo = new VersionInfoModel {ApplicationName = ApplicationConstants.ApplicationClientName};
            serverInfo = await _api.System_VersionAsync();
            apiVersions = await _api.System_AvailableApiVersionsAsync();
        }
    }
}