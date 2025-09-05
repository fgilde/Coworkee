using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using lib.Coworkee.Application.Common.Extensions;
using lib.Coworkee.Application.Common.Models.Chat;
using lib.Coworkee.Application.Common.Models.Identity;
using lib.Coworkee.Application.Contracts.Chat;
using lib.Coworkee.Application.Contracts.Services;
using lib.Coworkee.Shared.Constants.Permission;
using lib.Coworkee.Shared.Wrapper;
using lib.Coworkee.Application.Contracts.Services.Identity;
using lib.Coworkee.Application.Hubs.Events.Base;
using lib.Coworkee.Application.Requests;
using Coworkee.Shared;
using Microsoft.Extensions.Localization;

namespace Coworkee.Server.Controllers.Communication
{
    [Authorize(Policy = Permissions.Communication.Chat)]
    [ApiController]
    public class ChatsController : BaseApiController<ChatsController>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IChatService _chatService;

        public ChatsController(ICurrentUserService currentUserService, IChatService chatService)
        {
            _currentUserService = currentUserService;
            _chatService = chatService;
        }

        /// <summary>
        /// Deletes this message
        /// </summary>
        [HttpDelete("{messageId}")]
        [Produces(typeof(Result<IEnumerable<ChatHistoryResponse>>))]
        public async Task<IActionResult> DeleteMessageAsync(long messageId)
        {
            return Ok(await _chatService.DeleteMessageAsync(messageId, _currentUserService.UserId));
        }

        /// <summary>
        /// Get user wise chat history
        /// </summary>
        /// <param name="contactId"></param>
        /// <returns>Status 200 OK</returns>
        //Get user wise chat history
        [HttpGet("{contactId}")]
        [Produces(typeof(Result<IEnumerable<ChatHistoryResponse>>))]
        public async Task<IActionResult> GetChatHistoryAsync(string contactId)
        {
            return Ok(await _chatService.GetChatHistoryAsync(_currentUserService.UserId, contactId));
        }
        /// <summary>
        /// get available users
        /// </summary>
        /// <returns>Status 200 OK</returns>
        //get available users - sorted by date of last message if exists
        [HttpGet("users")]
        [Produces(typeof(Result<IEnumerable<ChatUserResponse>>))]
        public async Task<IActionResult> GetChatUsersAsync()
        {
            return Ok(await _chatService.GetChatUsersAsync(_currentUserService.UserId));
        }

        /// <summary>
        /// Save Chat Message
        /// </summary>
        /// <param name="message"></param>
        /// <returns>Status 200 OK</returns>
        //save chat message
        [HttpPost]
        [Produces(typeof(Result))]
        public async Task<IActionResult> SaveMessageAsync(ChatHistory<IChatUser> message)
        {
            message.FromUserId = _currentUserService.UserId;
            message.ToUserId = message.ToUserId;
            message.CreatedDate = DateTime.Now;
            await NotifyRecipientAsync(message);
            return Ok(await _chatService.SaveMessageAsync(message));
        }

        private async Task NotifyRecipientAsync(ChatHistory<IChatUser> message)
        {
            using var scope = Get<IUserCultureScopeService>().CreateUserCultureScope(message.ToUserId);

            var localizer = Get<IStringLocalizer<ChatsController>>();
            var sender = _currentUserService.Principal;
            var name = sender.GetFullName();
            var mailContent = await Get<IEmailTemplateService>().RunAsync(EmailTemplate.NewChatMessage(), new { Message = message.Message, SenderName = name, SenderId = sender.GetUserId() });
            await ClientEventHub.Clients.Group(message.ToUserId).ReceiveMessage(message, name);
            await Get<INotificationService>().SendAsync(new NotificationRequest()
            {
                PersistInDb = true,
                SendAsMail = NotificationAsMail.WhenTargetOffline,
                Url = $"/chat/{sender.GetUserId()}",
                Subject = localizer["New Message From {0}", name],
                Content = localizer["You received a new chat message from {0}", name],
                HtmlContent = mailContent,
                Target = EventTarget.User(message.ToUserId)
            });
        }
    }
}