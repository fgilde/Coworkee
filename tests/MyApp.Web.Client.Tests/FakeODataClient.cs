using Coworkee.Client.Blazor.Data;
using Coworkee.Contracts.Data;

namespace MyApp.Web.Client.Tests;

internal sealed class FakeODataClient : IODataClient
{
    private readonly Dictionary<string, object> _sets = new(StringComparer.Ordinal);

    public FakeODataClient With<T>(string entitySet, params T[] items)
    {
        _sets[entitySet] = items;
        return this;
    }

    public Task<ODataPage<T>> QueryAsync<T>(string entitySet, ODataQuery query, CancellationToken cancellationToken = default)
    {
        var items = _sets.TryGetValue(entitySet, out var set) ? (T[])set : [];
        return Task.FromResult(new ODataPage<T>(items, items.Length, []));
    }

    public Task<byte[]> ExportAsync(string entitySet, ODataQuery query, CancellationToken cancellationToken = default) => Task.FromResult<byte[]>([]);

    public Task<ImportResult> ImportAsync(string entitySet, Stream workbook, string fileName, CancellationToken cancellationToken = default) =>
        Task.FromResult(new ImportResult(0, []));
}
