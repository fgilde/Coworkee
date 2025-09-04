using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;
using System;
using Coworkee.Application.Common.Models;
using Coworkee.Client.JsInterop;
using Coworkee.Client.Models;
using Microsoft.JSInterop;
using System.Linq;

namespace Coworkee.Client.Shared.Components
{
    public partial class Assistant
    {
        [Parameter] public string CurrentMessage { get; set; }
        private List<AssistantCommandClientModel> Messages { get; } = new()
        {
            //new AssistantCommandModel { Owner = AssistantCommandOwner.Assistant, Completed = true, Message = "Hello, I'm Coworkee Assistant. How can I help you?" },
        };
        private bool _canSubmit => !_isThinking && !string.IsNullOrWhiteSpace(CurrentMessage);
        private bool _isThinking = false;

        private async Task OnKeyUpInChat(KeyboardEventArgs e)
        {
            if (e.Key == "Enter" && _canSubmit && !e.ShiftKey && !e.AltKey)
            {
                await SubmitAsync();
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

        private async Task ScrollToBottomAsync()
        {
            await _jsRuntime.InvokeAsync<string>(JsNamespace.Get("BrowserHelper", "scrollToBottom"), "assistantChatContainer");
            StateHasChanged();
        }

        private async Task SubmitAsync()
        {
            _isThinking = true;
            var question = new AssistantCommandClientModel
            {
                Owner = AssistantCommandOwner.User,
                Message = CurrentMessage
            };
            Messages.Add(question);
            CurrentMessage = null;
            AssistantCommandClientModel result = null;
            await foreach (var s in (await _api.Assistant_AskWithHistoryAsync(Messages.Cast<AssistantCommandDto>().ToList())).ReadAsStringStreamAsync(10))
            {
                Console.WriteLine(s);
                if (result == null)
                {
                    Messages.Add(result = new AssistantCommandClientModel
                    {
                        Owner = AssistantCommandOwner.Assistant,
                        Message = s
                    });
                }
                else
                {
                    result.Message += s;
                }
                StateHasChanged();
                await ScrollToBottomAsync();
            }

            if (result != null)
                result.Completed = true;
            question.Completed = true;
            _isThinking = false;
            await ScrollToBottomAsync();
        }


    }

}
