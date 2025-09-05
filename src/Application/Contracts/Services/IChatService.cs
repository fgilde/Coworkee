using System.Collections.Generic;
using System.Threading.Tasks;
using lib.Coworkee.Application.Common.Models.Chat;
using lib.Coworkee.Application.Common.Models.Identity;
using lib.Coworkee.Application.Contracts.Chat;
using lib.Coworkee.Shared.Wrapper;

namespace Coworkee.Application.Contracts.Services
{
    public interface IChatService
    {
        Task<Result<IEnumerable<ChatUserResponse>>> GetChatUsersAsync(string userId);

        Task<IResult> SaveMessageAsync(ChatHistory<IChatUser> message);

        Task<Result<IEnumerable<ChatHistoryResponse>>> GetChatHistoryAsync(string userId, string contactId);
        Task<Result<IEnumerable<ChatHistoryResponse>>> DeleteMessageAsync(long messageId, string currentUserId);
    }
}