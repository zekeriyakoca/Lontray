using Basket.API.Model;
using Cache.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.Infrastructure.Repositories
{
    public class RedisBasketRepository : IBasketRepository
    {
        internal static string Prefix = "Basket.API-";
        private readonly ICacheService cache;

        public RedisBasketRepository(ICacheService cache)
        {
            this.cache = cache;
        }

        public async Task DeleteBasketAsync(string customerId)
        {
            await cache.RemoveValue(customerId.ToRedisKey());
        }

        public async Task<CustomerBasket> GetBasketAsync(string customerId)
        {
            return await cache.GetValue<CustomerBasket>(customerId.ToRedisKey());
        }

        public IEnumerable<string> GetUsers()
        {
            // TODO : Evaluate caching users for a certain period of time
            return cache.GetKeys($"{Prefix}*").Select(k => k.UnwrapRedisPrefix());
        }

        public async Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket)
        {
            await cache.SetValue<CustomerBasket>(basket.CustomerId.ToRedisKey(), basket);
            return basket; // TODO : Evaluate returning data from cache
        }
    }
    internal static class StringExtensions
    {
        internal static string Prefix = "Basket.API-";
        internal static string ToRedisKey(this string key)
        {
            return string.Concat(Prefix, key);
        }
        internal static string UnwrapRedisPrefix(this string key)
        {
            return key.Replace(Prefix, "");
        }

    }
}
