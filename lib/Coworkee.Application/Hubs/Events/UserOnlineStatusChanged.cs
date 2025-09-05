using Coworkee.Application.Common.Models.Identity;

namespace Coworkee.Application.Hubs.Events;

public class UserOnlineStatusChanged : UserProfileChanged
{
    public UserOnlineStatusChanged()
    {}

    public UserOnlineStatusChanged(UserResponse user) : base(user)
    {}
}