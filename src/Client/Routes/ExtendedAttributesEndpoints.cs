namespace Coworkee.Client.Routes
{
    public static class ExtendedAttributesEndpoints
    {
        public static string GetAll(string entityName) => $"{BaseEndpoints.Api}/{entityName}ExtendedAttributes";
        public static string GetAllByEntityId<TEntityId>(string entityName, TEntityId entityId) => $"{GetAll(entityName)}/by-entity/{entityId}";
        public static string Export<TEntityId>(string entityName, TEntityId entityId, bool includeEntity, bool onlyCurrentGroup = false, string currentGroup = "") => $"{BaseEndpoints.Api}/{entityName}ExtendedAttributes/export?{nameof(entityId)}={entityId}&{nameof(includeEntity)}={includeEntity}&{nameof(onlyCurrentGroup)}={onlyCurrentGroup}&{nameof(currentGroup)}={currentGroup}";
        public static string ExportFiltered<TEntityId>(string entityName, string searchString, TEntityId entityId, bool includeEntity, bool onlyCurrentGroup = false, string currentGroup = "") => $"{BaseEndpoints.Api}/{entityName}ExtendedAttributes/export?{nameof(searchString)}={searchString}&{nameof(entityId)}={entityId}&{nameof(includeEntity)}={includeEntity}&{nameof(onlyCurrentGroup)}={onlyCurrentGroup}&{nameof(currentGroup)}={currentGroup}";
        public static string Delete(string entityName) => $"{BaseEndpoints.Api}/{entityName}ExtendedAttributes";
        public static string Save(string entityName) => $"{BaseEndpoints.Api}/{entityName}ExtendedAttributes";
        public static string GetCount(string entityName) => $"{BaseEndpoints.Api}/{entityName}ExtendedAttributes/count";
    }
}