using Coworkee.Client.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coworkee.Application.Common.Extensions;
using Coworkee.Application.Common.Models.Chat;
using Coworkee.Application.Common.Models.Identity;
using Coworkee.Application.Contracts.Chat;
using Coworkee.Application.Contracts.Hubs;
using Coworkee.Client.JsInterop;
using Coworkee.SDK;
using Coworkee.Shared.Constants.Storage;

namespace Coworkee.Client.Pages.Communication
{
    public partial class Chat: IAsyncDisposable
    {
        [Inject] private IApplicationClient Api { get; set; }

        [CascadingParameter] private HubConnection HubConnection { get; set; }
        [Parameter] public string CurrentMessage { get; set; }
        [Parameter] public string CurrentUserId { get; set; }
        [Parameter] public string CurrentUserImageURL { get; set; }

        public List<ChatUserResponse> UserList = new();
        [Parameter] public string CFullName { get; set; }
        [Parameter] public string CId { get; set; }
        [Parameter] public string CUserName { get; set; }
        [Parameter] public string CImageURL { get; set; }

        private List<ChatHistoryResponse> _messages = new();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await ScrollToBottomAsync();
        }
        
        protected override async Task OnInitializedAsync()
        {
            HubConnection = await HubConnection.EnsureStartedAsync(_config.BackendOrigin);

            HubConnection.On<string>(nameof(IClientEventHub.ConnectUser), (userId) =>
            {
                var connectedUser = UserList.Find(x => x.Id.Equals(userId));
                if (connectedUser is {IsOnline: false})
                {
                    connectedUser.IsOnline = true;
                    _snackBar.Add($"{connectedUser.UserName} {_localizer["Logged In."]}", Severity.Info);
                    StateHasChanged();
                }
            });
            HubConnection.On<string>(nameof(IClientEventHub.DisconnectUser), (userId) =>
            {
                var disconnectedUser = UserList.Find(x => x.Id.Equals(userId));
                if (disconnectedUser is {IsOnline: true})
                {
                    disconnectedUser.IsOnline = false;
                    _snackBar.Add($"{disconnectedUser.UserName} {_localizer["Logged Out."]}", Severity.Info);
                    StateHasChanged();
                }
            });
            HubConnection.On<ChatHistory<IChatUser>, string>(nameof(IClientEventHub.ReceiveMessage), async (chatHistory, userName) =>
             {
                 if (CId == chatHistory.FromUserId)
                     await AddMessageAsync(new ChatHistoryResponse { Message = chatHistory.Message, FromUserId = chatHistory.FromUserId, FromUserFullName = userName, CreatedDate = chatHistory.CreatedDate, FromUserImageURL = CImageURL });
                 
             });
            await GetUsersAsync();
            var state = await _stateProvider.GetAuthenticationStateAsync();
            var user = state.User;
            CurrentUserId = user.GetUserId();
            CurrentUserImageURL = await _localStorage.GetItemAsync<string>(StorageConstants.Local.UserImageURL);
            if (!string.IsNullOrEmpty(CId))
            {
                await LoadUserChat(CId);
            }
        }

        private async Task ScrollToBottomAsync()
        {
            await _jsRuntime.InvokeAsync<string>(JsNamespace.Get("BrowserHelper","scrollToBottom"), "chatContainer");
        }

        private async Task SubmitAsync()
        {
            if (!string.IsNullOrEmpty(CurrentMessage) && !string.IsNullOrEmpty(CId))
            {
                //Save Message to DB
                var chatHistory = new ChatHistory<IChatUser>()
                {
                    Message = CurrentMessage,
                    ToUserId = CId,
                    CreatedDate = DateTime.Now
                };
                var response = await Api.Chats_SaveMessageAsync(chatHistory);
                if (_errorService.IsSuccessFull(response))
                {
                    var state = await _stateProvider.GetAuthenticationStateAsync();
                    var user = state.User;
                    CurrentUserId = user.GetUserId();
                    chatHistory.FromUserId = CurrentUserId;
                    await AddMessageAsync(new ChatHistoryResponse { Message = chatHistory.Message, FromUserId = chatHistory.FromUserId, FromUserFullName = user.GetFullName(), CreatedDate = chatHistory.CreatedDate, FromUserImageURL = CImageURL });
                    CurrentMessage = string.Empty;
                }
            }
        }

        private async Task OnKeyPressInChat(KeyboardEventArgs e)
        {
            if (e.Key == "Enter" && !e.ShiftKey && !e.AltKey)
                await SubmitAsync();
        }

        private Task OnKeyDownInChat(KeyboardEventArgs e)
        {
            if (e.Key == "Escape")
            {
                CurrentMessage = string.Empty;
                StateHasChanged();
            }

            return Task.CompletedTask;
        }

        private async Task LoadUserChat(string userId)
        {
            _open = false;
            var response = await _api.User_GetByIdAsync(userId);
            if (_errorService.IsSuccessFull(response))
            {
                var contact = response.Data;
                CId = contact.Id;
                CFullName = $"{contact.FirstName} {contact.LastName}";
                CUserName = contact.UserName;
                CImageURL = contact.ProfilePictureDataUrl;
                _navigationManager.NavigateTo($"chat/{CId}");
                //Load messages from db here
                _messages = new List<ChatHistoryResponse>();
                var historyResponse = await Api.Chats_GetChatHistoryAsync(CId);
                if (_errorService.IsSuccessFull(historyResponse))
                {
                    _messages = historyResponse.Data.ToList();
                }
            }
        }

        private async Task AddMessageAsync(ChatHistoryResponse message)
        {
            _messages.Add(message);
            await ScrollToBottomAsync();
            StateHasChanged();
        }

        private async Task GetUsersAsync()
        {
            //add get chat history from chat controller / manager
            var response = await Api.Chats_GetChatUsersAsync();
            if (_errorService.IsSuccessFull(response))
            {
                UserList = response.Data.ToList();
            }
        }

        private bool _open;
        private Anchor ChatDrawer { get; set; }

        private void OpenDrawer(Anchor anchor)
        {
            ChatDrawer = anchor;
            _open = true;
        }

        public ValueTask DisposeAsync()
        {
            return HubConnection.TryDisposeAsync();
        }
    }
}