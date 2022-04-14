using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PaperlessClient.Mobile.Services.Abstraction
{
    public interface IPersistenceService
    {
        Task<bool> PersistAsync<T>(string key, T save);
        Task DeleteAsync(string key);
        Task<bool> PersistSecureAsync<T>(string key, T save);
        Task<T> GetOrFetchObjectAsync<T>(string key, Func<Task<T>> fetchFunc, DateTimeOffset? absoluteExpiration = null);
        IObservable<T> GetAndFetchObjectAsync<T>(string key, Func<Task<T>> fetchFunc);
        Task<T> GetAsync<T>(string key);
        Task<List<T>> GetAllAsync<T>();
        Task<T> GetSecureAsync<T>(string key);
        Task<List<T>> GetAllSecureAsync<T>();
        Task DeleteSecureAsync(string key);
    }
}
