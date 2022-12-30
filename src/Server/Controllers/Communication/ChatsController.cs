using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Coworkee.Application.Common.Extensions;
using Coworkee.Application.Common.Models.Chat;
using Coworkee.Application.Common.Models.Identity;
using Coworkee.Application.Contracts.Chat;
using Coworkee.Application.Contracts.Services;
using Coworkee.Shared.Constants.Permission;
using Coworkee.Shared.Wrapper;

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
            var name = _currentUserService.Principal.GetFullName();
            await ClientEventHub.Clients.Group(message.ToUserId).ReceiveMessage(message, name);
            return Ok(await _chatService.SaveMessageAsync(message));
        }
    }
}