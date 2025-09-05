using System.Threading.Tasks;

namespace lib.Coworkee.Application.Contracts.Services.Identity;

public interface IDictionaryService
{
    public Task<object> GetAsync(string key);
    public Task SetAsync(string key, object value);
    public Task RemoveAsync(string key);
    public Task<bool> ExistsAsync(string key);
    public Task<T> GetAsAsync<T>(string key);
}