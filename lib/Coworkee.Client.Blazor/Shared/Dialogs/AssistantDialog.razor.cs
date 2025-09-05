
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace lib.Coworkee.Client.Shared.Dialogs
{
    public partial class AssistantDialog
    {
        [Parameter] public string CurrentMessage { get; set; }
        [Parameter] public string Title { get; set; } = "Assistant";


        private async Task OnKeyUpInChat(KeyboardEventArgs e)
        {
            if (e.Key == "Enter" && !e.ShiftKey && !e.AltKey)
            {
              //  await SubmitAsync();
            }
        }

        private Task OnKeyDownInChat(KeyboardEventArgs e)
        {
            if (e.Key == "Escape")
            {
                CurrentMessage = null;
                StateHasChanged();
            }

            return Task.CompletedTask;
        }

    }


}
