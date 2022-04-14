using Akavache;
using PaperlessClient.Mobile.Services.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace PaperlessClient.Mobile.Services
{
    public class PersistenceService : IPersistenceService
    {
        ISecureBlobCache _secure = BlobCache.Secure;
        IBlobCache _cache = BlobCache.UserAccount;

        public PersistenceService()
        {
            BlobCache.ForcedDateTimeKind = DateTimeKind.Local; // Fix time issues
        }

        public async Task<T> GetAsync<T>(string key)
        {
            T obj = default(T);
            try
            {
                obj = await _cache.GetObject<T>(key);
            }
            catch (Exception)
            {
                return default(T);
            }
            return obj;
        }

        public async Task<List<T>> GetAllAsync<T>()
        {
            return (await _cache.GetAllObjects<T>()).ToList();
        }

        public async Task<T> GetSecureAsync<T>(string key)
        {
            return await _secure.GetObject<T>(key);
        }

        public async Task<List<T>> GetAllSecureAsync<T>()
        {
            return (await _secure.GetAllObjects<T>()).ToList();
        }

        public async Task<T> GetOrFetchObjectAsync<T>(string key, Func<Task<T>> fetchFunc, DateTimeOffset? absoluteExpiration = null)
        {
            return await _cache.GetOrFetchObject<T>(key, fetchFunc, absoluteExpiration);
        }

        public IObservable<T> GetAndFetchObjectAsync<T>(string key, Func<Task<T>> fetchFunc)
        {
            return _cache.GetAndFetchLatest<T>(key, fetchFunc);
        }

        public async Task<bool> PersistAsync<T>(string key, T save)
        {
            try
            {
                await _cache.InsertObject(key, save, TimeSpan.FromDays(90));
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public async Task DeleteAsync(string key)
        {
            await _cache.Invalidate(key);
        }

        public async Task<bool> PersistSecureAsync<T>(string key, T save)
        {
            try
            {
                await _secure.InsertObject(key, save, TimeSpan.FromDays(90));
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public async Task DeleteSecureAsync(string key)
        {
            await _secure.Invalidate(key);
        }
    }
}
