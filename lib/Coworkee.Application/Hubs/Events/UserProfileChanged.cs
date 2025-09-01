using Coworkee.Application.Common.Models.Identity;
using Coworkee.Application.Hubs.Events.Base;

namespace Coworkee.Application.Hubs.Events;

public class UserProfileChanged : ClientEventBase
{
    public UserProfileChanged()
    {
        Target = EventTarget.All;
    }

    public UserProfileChanged(UserResponse user) : this()
    {
        User = user;
    }

    public UserResponse User { get; set; }
}