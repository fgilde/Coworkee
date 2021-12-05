using System.Collections.Generic;
using CleanArchitectureBase.Application.Common.Models.Identity;

namespace CleanArchitectureBase.Application.Hubs.Events
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