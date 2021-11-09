using CleanArchitectureBase.Application.Hubs.Events.Base;
using CleanArchitectureBase.Application.Responses.Identity;

namespace CleanArchitectureBase.Application.Hubs.Events
{
    public class EntitiesUpdated<TDto> : ClientEventBase
    {
        public EntitiesUpdated()
        {
            Target = EventTarget.All;
        }

        public EntitiesUpdated(UserResponse user, TDto[] entities): this()
        {
            Entities = entities;
            User = user;
        }

        public UserResponse User { get; set; }

        public TDto[] Entities { get; set; }
    }
}