using System.Collections.Generic;
using System.Security.Claims;
using CleanArchitectureBase.Application.Common.Models.Identity;

namespace CleanArchitectureBase.Application.Hubs.Events
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