using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cache.Services
{
    public class InMemmoryCacheService : ICacheService
    {
        private readonly IMemoryCache cache;

        public InMemmoryCacheService(IMemoryCache cache)
        {
            this.cache = cache;
        }

        public Task<string> GetValue(string key)
        {
            cache.TryGetValue(key, out string value);
            return Task.FromResult(value);
        }

        public Task<T> GetValue<T>(string key)
        {
            cache.TryGetValue(key, out dynamic value);
            if (value == null)
                return Task.FromResult<T>(default(T));
            dynamic result = value.GetType() == typeof(T) ? value : JsonConvert.DeserializeObject<T>(value);
            return Task.FromResult<T>(result);
        }

        public Task RemoveValue(string key)
        {
            cache.Remove(key);
            return Task.CompletedTask;
        }

        public Task SetValue(string key, object myObject)
        {
            if (String.IsNullOrWhiteSpace(key))
            {
                return Task.FromException(new ArgumentNullException("Key value is required"));
            }
            cache.Set(key, JsonConvert.SerializeObject(myObject), TimeSpan.FromSeconds(600));
            return Task.CompletedTask;
        }

        public Task SetValue<T>(string key, T myObject)
        {
            if (String.IsNullOrWhiteSpace(key))
            {
                return Task.FromException(new ArgumentNullException("Key value is required"));
            }
            cache.Set(key, JsonConvert.SerializeObject(myObject), TimeSpan.FromSeconds(600));
            return Task.CompletedTask;
        }

        public Task SetValue<T>(string key, T myObject, int second)
        {
            if (String.IsNullOrWhiteSpace(key))
            {
                return Task.FromException(new ArgumentNullException("Key value is required"));
            }
            cache.Set(key, JsonConvert.SerializeObject(myObject), TimeSpan.FromSeconds(second));
            return Task.CompletedTask;
        }

        public IEnumerable<string> GetKeys(string pattern)
        {
            throw new NotImplementedException();
        }
    }
}
