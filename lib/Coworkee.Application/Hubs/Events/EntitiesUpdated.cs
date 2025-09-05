using System.Collections.Generic;
using System.Linq;
using Coworkee.Application.Common.Models.Identity;
using Coworkee.Application.Hubs.Events.Base;

namespace Coworkee.Application.Hubs.Events
{
    public class EntitiesUpdated<TDto> : EntitiesUpdated
    {
        public EntitiesUpdated(){}

        public EntitiesUpdated(UserResponse user, IEnumerable<TDto> entities): this()
        {
            Entities = entities.ToArray(); // we need to enumerate class will be transferred
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

        public EntitiesUpdated(UserResponse user, IEnumerable<string> idsAsString) : this()
        {
            User = user;
            IdsAsString = idsAsString.Where(s => !string.IsNullOrEmpty(s)).ToArray(); // we need to enumerate class will be transferred
        }

        public string[] IdsAsString { get; set; }
        public UserResponse User { get; set; }
    }
}