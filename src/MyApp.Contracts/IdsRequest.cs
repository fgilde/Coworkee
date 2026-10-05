namespace MyApp.Contracts;

public sealed record IdsRequest(IReadOnlyList<Guid> Ids);
