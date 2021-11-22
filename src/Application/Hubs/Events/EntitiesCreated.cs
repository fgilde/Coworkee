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

    public class EntitiesCreated : EntitiesUpdated
    {
        public EntitiesCreated()
        { }

        public EntitiesCreated(UserResponse user, string[] idsAsString) : base(user, idsAsString)
        { }
    }
}