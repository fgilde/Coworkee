using CleanArchitectureBase.Application.Common.Models.Identity;
using CleanArchitectureBase.Application.Hubs.Events.Base;

namespace CleanArchitectureBase.Application.Hubs.Events;

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