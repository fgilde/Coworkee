using CleanArchitectureBase.Shared.Constants.Application;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace CleanArchitectureBase.Client.Shared.Components
{
    public partial class AppBarHeader
    {
        private string _title = ApplicationConstants.ApplicationName;
        [Parameter] public EventCallback<MouseEventArgs> OnMenuIconClick { get; set; }
        [Parameter] public MenuTogglePosition MenuTogglePosition { get; set; } = MenuTogglePosition.End;
        [Parameter] public TitleBehaviour TitleBehaviour { get; set; } = TitleBehaviour.AppNameAndTitle;
        [Parameter] public bool IsLogoVisible { get; set; } = true;

        [Parameter]
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                _jsRuntime.InvokeVoidAsync("SetTitle", value);
            }
        }

        private string GetRenderTitle()
        {
            if (TitleBehaviour == TitleBehaviour.AppNameOnly)
                return ApplicationConstants.ApplicationName;
            if (Title != ApplicationConstants.ApplicationName && TitleBehaviour == TitleBehaviour.AppNameAndTitle)
                return $"{ApplicationConstants.ApplicationName} - {Title}";
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

    public enum MenuTogglePosition
    {
        Start,
        End,
        Hidden
    }

    public enum TitleBehaviour 
    {
        TitleOnly,
        AppNameOnly,
        AppNameAndTitle,
        Hidden
    }
}