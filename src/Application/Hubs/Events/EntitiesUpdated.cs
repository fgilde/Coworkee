using System.Linq;
using CleanArchitectureBase.Application.Hubs.Events.Base;
using CleanArchitectureBase.Application.Responses.Identity;

namespace CleanArchitectureBase.Application.Hubs.Events
{
    public class EntitiesUpdated<TDto> : EntitiesUpdated
    {
        public EntitiesUpdated(){}

        public EntitiesUpdated(UserResponse user, TDto[] entities): this()
        {
            Entities = entities;
            User = user;
        }

        public TDto[] Entities { get; set; }
    }

    public class EntitiesUpdated : ClientEventBase
    {
        public EntitiesUpdated()
        {
            Target = EventTarget.All;
        }

        public EntitiesUpdated(UserResponse user, string[] idsAsString) : this()
        {
            User = user;
            IdsAsString = idsAsString;
        }

        public string[] IdsAsString { get; set; }
        public UserResponse User { get; set; }
    }
}