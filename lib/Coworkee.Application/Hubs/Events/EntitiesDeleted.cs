using System.Collections.Generic;
using Coworkee.Application.Common.Models.Identity;

namespace Coworkee.Application.Hubs.Events
{
    public class EntitiesDeleted<TDto>: EntitiesUpdated<TDto>
    {
        public EntitiesDeleted()
        {}

        public EntitiesDeleted(UserResponse user, IEnumerable<TDto> entities) : base(user, entities)
        {}
    }

    public class EntitiesDeleted : EntitiesUpdated
    {
        public EntitiesDeleted()
        { }

        public EntitiesDeleted(UserResponse user, string[] idsAsString) : base(user, idsAsString)
        { }
    }
}