using System.Collections.Generic;
using System.Threading.Tasks;
using lib.Coworkee.Application.Contracts.Services.Identity;
using Microsoft.Extensions.DependencyInjection;
using Nextended.Core.Attributes;

namespace lib.Coworkee.Infrastructure.Services.Identity;

[RegisterAs(typeof(IDictionaryService), 1, RegisterAsImplementation = true, ServiceLifetime = ServiceLifetime.Singleton)]
public class DictionaryService: IDictionaryService
{
    private readonly IDictionary<string, object> _dictionary = new Dictionary<string, object>();
    public Task<object> GetAsync(string key) => Task.FromResult(_dictionary[key]);

    public Task SetAsync(string key, object value)
    {
        _dictionary[key] = value;
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key)
    {
        _dictionary.Remove(key);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string key)
    {
        return Task.FromResult(_dictionary.ContainsKey(key));
    }

    public Task<T> GetAsAsync<T>(string key)
    {
        return Task.FromResult((T)_dictionary[key]);
    }
}