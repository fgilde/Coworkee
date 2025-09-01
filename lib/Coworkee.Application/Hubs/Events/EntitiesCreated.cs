using System.Collections.Generic;
using Coworkee.Application.Common.Models.Identity;

namespace Coworkee.Application.Hubs.Events
{
    public class EntitiesCreated<TDto>: EntitiesUpdated<TDto>
    {
        public EntitiesCreated()
        {}

        public EntitiesCreated(UserResponse user, IEnumerable<TDto> entities) : base(user, entities)
        {}
    }

    public class EntitiesCreated : EntitiesUpdated
    {
        public EntitiesCreated()
        { }

        public EntitiesCreated(UserResponse user, IEnumerable<string> idsAsString) : base(user, idsAsString)
        { }
    }
}