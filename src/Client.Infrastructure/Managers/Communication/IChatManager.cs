using CleanArchitectureBase.Application.Models.Chat;
using CleanArchitectureBase.Application.Responses.Identity;
using CleanArchitectureBase.Shared.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Interfaces.Chat;

namespace CleanArchitectureBase.Client.Infrastructure.Managers.Communication
{
    public interface IChatManager : IManager
    {
        Task<IResult<IEnumerable<ChatUserResponse>>> GetChatUsersAsync();

        Task<IResult> SaveMessageAsync(ChatHistory<IChatUser> chatHistory);

        Task<IResult<IEnumerable<ChatHistoryResponse>>> GetChatHistoryAsync(string cId);
    }
}