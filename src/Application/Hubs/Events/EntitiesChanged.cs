using System.Security.Claims;
using CleanArchitectureBase.Application.Responses.Identity;

namespace CleanArchitectureBase.Application.Hubs.Events
{
    public class EntitiesChanged<TDto> : EntitiesUpdated<TDto>
    {
        public EntitiesChanged()
        {}

        public EntitiesChanged(UserResponse user, TDto[] entities) : base(user, entities)
        {}
    }
}