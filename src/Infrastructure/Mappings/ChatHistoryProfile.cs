using AutoMapper;
using CleanArchitectureBase.Application.Interfaces.Chat;
using CleanArchitectureBase.Application.Models.Chat;
using CleanArchitectureBase.Infrastructure.Models.Identity;

namespace CleanArchitectureBase.Infrastructure.Mappings
{
    public class ChatHistoryProfile : Profile
    {
        public ChatHistoryProfile()
        {
            CreateMap<ChatHistory<IChatUser>, ChatHistory<BlazorHeroUser>>().ReverseMap();
        }
    }
}