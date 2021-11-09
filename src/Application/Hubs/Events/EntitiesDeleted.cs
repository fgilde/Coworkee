using CleanArchitectureBase.Application.Responses.Identity;

namespace CleanArchitectureBase.Application.Hubs.Events
{
    public class EntitiesDeleted<TDto>: EntitiesUpdated<TDto>
    {
        public EntitiesDeleted()
        {}

        public EntitiesDeleted(UserResponse user, TDto[] entities) : base(user, entities)
        {}
    }
}