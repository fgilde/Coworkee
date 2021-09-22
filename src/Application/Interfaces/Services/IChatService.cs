using CleanArchitectureBase.Application.Responses.Identity;
using CleanArchitectureBase.Shared.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Interfaces.Chat;
using CleanArchitectureBase.Application.Models.Chat;

namespace CleanArchitectureBase.Application.Interfaces.Services
{
    public interface IChatService
    {
        Task<Result<IEnumerable<ChatUserResponse>>> GetChatUsersAsync(string userId);

        Task<IResult> SaveMessageAsync(ChatHistory<IChatUser> message);

        Task<Result<IEnumerable<ChatHistoryResponse>>> GetChatHistoryAsync(string userId, string contactId);
    }
}