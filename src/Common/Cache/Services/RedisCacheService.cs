using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cache.Services
{
    public class RedisCacheService : ICacheService
    {
        static string configString;
        readonly Lazy<ConnectionMultiplexer> redis = new Lazy<ConnectionMultiplexer>(InitializeMultiplexer);

        IDatabase Database { get { return redis.Value.GetDatabase(); } }

        public RedisCacheService(IConfiguration configuration)
        {
            configString = configuration["Redis:ConnectionService"];
        }

        static ConnectionMultiplexer InitializeMultiplexer()
        {
            var conf = ConfigurationOptions.Parse(configString);
            var multiplexer = ConnectionMultiplexer.Connect(conf);
            return multiplexer;
        }


        public async Task<string> GetValue(string key)
        {
            return await Database.StringGetAsync(key);
        }

        public async Task RemoveValue(string key)
        {
            await Database.KeyDeleteAsync(key);
        }

        public async Task SetValue(string key, object myObject)
        {
            await Database.StringSetAsync(key, JsonConvert.SerializeObject(myObject));
        }

        public async Task SetValue<T>(string key, T myObject)
        {
            await Database.StringSetAsync(key, JsonConvert.SerializeObject(myObject));
        }

        public async Task SetValue<T>(string key, T myObject, int second)
        {
            await Database.StringSetAsync(key, JsonConvert.SerializeObject(myObject), TimeSpan.FromSeconds(second));
        }

        public async Task<T> GetValue<T>(string key)
        {
            var value = await Database.StringGetAsync(key);
            if (value.IsNull)
                return default;
            return JsonConvert.DeserializeObject<T>(value);
        }

        public IEnumerable<string> GetKeys(string pattern)
        {
            var server = GetServer(); // TODO : Modify for multiple instance Redis
            var data = String.IsNullOrWhiteSpace(pattern) ? server.Keys() : server.Keys(pattern: pattern);

            return data?.Select(k => k.ToString());
        }

        private IServer GetServer()
        {
            var endpoints = redis.Value.GetEndPoints();
            return redis.Value.GetServer(endpoints.First());
        }
    }
}
