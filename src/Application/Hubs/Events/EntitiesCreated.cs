using System.Security.Claims;
using CleanArchitectureBase.Application.Responses.Identity;

namespace CleanArchitectureBase.Application.Hubs.Events
{
    public class EntitiesCreated<TDto>: EntitiesUpdated<TDto>
    {
        public EntitiesCreated()
        {}

        public EntitiesCreated(UserResponse user, TDto[] entities) : base(user, entities)
        {}
    }
}