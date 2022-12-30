using System.Collections.Generic;
using System.Threading.Tasks;
using Coworkee.Application.Common.Models.Chat;
using Coworkee.Application.Common.Models.Identity;
using Coworkee.Application.Contracts.Chat;
using Coworkee.Shared.Wrapper;

namespace Coworkee.Application.Contracts.Services
{
    public interface IChatService
    {
        Task<Result<IEnumerable<ChatUserResponse>>> GetChatUsersAsync(string userId);

        Task<IResult> SaveMessageAsync(ChatHistory<IChatUser> message);

        Task<Result<IEnumerable<ChatHistoryResponse>>> GetChatHistoryAsync(string userId, string contactId);
    }
}