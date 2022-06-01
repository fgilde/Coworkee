using CleanArchitectureBase.Application.Common.Models.Identity;

namespace CleanArchitectureBase.Application.Hubs.Events;

public class UserOnlineStatusChanged : UserProfileChanged
{
    public UserOnlineStatusChanged()
    {}

    public UserOnlineStatusChanged(UserResponse user) : base(user)
    {}
}