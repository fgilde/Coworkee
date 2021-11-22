using System.Security.Claims;
using CleanArchitectureBase.Application.Common.Models.Identity;

namespace CleanArchitectureBase.Application.Hubs.Events
{
    public class EntitiesChanged<TDto> : EntitiesUpdated<TDto>
    {
        public EntitiesChanged()
        {}

        public EntitiesChanged(UserResponse user, TDto[] entities) : base(user, entities)
        {}
    }

    public class EntitiesChanged : EntitiesUpdated
    {
        public EntitiesChanged()
        { }

        public EntitiesChanged(UserResponse user, string[] idsAsString) : base(user, idsAsString)
        { }
    }
}