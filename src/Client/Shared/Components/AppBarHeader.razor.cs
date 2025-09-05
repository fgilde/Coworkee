using Coworkee.Client.Enums;
using Coworkee.Client.JsInterop;
using lib.Coworkee.Shared.Constants.Application;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Coworkee.Client.Shared.Components
{
    public partial class AppBarHeader
    {
        private string _title = ApplicationConstants.ApplicationName;
        [Parameter] public EventCallback<MouseEventArgs> OnMenuIconClick { get; set; }
        [Parameter] public MenuTogglePosition MenuTogglePosition { get; set; } = MenuTogglePosition.End;
        [Parameter] public AppBarTitleBehaviour TitleBehaviour { get; set; } = AppBarTitleBehaviour.AppNameAndTitle;
        [Parameter] public bool IsLogoVisible { get; set; } = true;

        [Parameter]
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                _jsRuntime.InvokeVoidAsync(JsNamespace.Get("BrowserHelper", "setTitle"), value);
            }
        }

        private string GetRenderTitle()
        {
            if (TitleBehaviour == AppBarTitleBehaviour.AppNameOnly)
                return ApplicationConstants.ApplicationName;
            if (Title != ApplicationConstants.ApplicationName && TitleBehaviour == AppBarTitleBehaviour.AppNameAndTitle)
                return !string.IsNullOrWhiteSpace(Title) ? $"{ApplicationConstants.ApplicationName} - {Title}" : ApplicationConstants.ApplicationName;
            return Title;
        }

        private async void MainClick()
        {
            if (MenuTogglePosition == MenuTogglePosition.Hidden)
            {
                await OnMenuIconClick.InvokeAsync();
            }
            else
            {
                _navigationManager.NavigateTo("/");
            }
        }
    }

 
}