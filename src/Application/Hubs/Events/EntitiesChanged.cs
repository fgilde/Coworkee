using System.Collections.Generic;
using System.Security.Claims;
using Coworkee.Application.Common.Models.Identity;

namespace Coworkee.Application.Hubs.Events
{
    public class EntitiesChanged<TDto> : EntitiesUpdated<TDto>
    {
        public EntitiesChanged()
        {}

        public EntitiesChanged(UserResponse user, IEnumerable<TDto> entities) : base(user, entities)
        {}
    }

    public class EntitiesChanged : EntitiesUpdated
    {
        public EntitiesChanged()
        { }

        public EntitiesChanged(UserResponse user, IEnumerable<string> idsAsString) : base(user, idsAsString)
        { }
    }
}