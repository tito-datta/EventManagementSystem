using StackExchange.Redis;
using System.Text.Json;

namespace data_access.redis.cache
{
    public class RedisCacheService<T> : ICacheAccess<T>, IDisposable where T : IRedisCacheEntity, new()
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _db;

        public RedisCacheService(string connectionString)
        {
            _redis = ConnectionMultiplexer.Connect(connectionString);
            _db = _redis.GetDatabase();
        }

        public async Task<T> GetAsync(string hashKey, string fieldId)
        {
            var value = await _db.HashGetAsync(hashKey, fieldId);
            if (value.IsNull)
                return default;

            return JsonSerializer.Deserialize<T>(value);
        }

        public async Task SetAsync(T entity)
        {
            var serializedValue = JsonSerializer.Serialize(entity.Value);
            await _db.HashSetAsync(entity.Id, [entity.CacheEntry]);
        }

        public async Task<bool> DeleteAsync(string hashKey, string fieldId) => await _db.HashDeleteAsync(hashKey, fieldId);

        public async Task<bool> ExistsAsync(string hashKey, string fieldId) => await _db.HashExistsAsync(hashKey, fieldId);

        public void Dispose() => _redis.Dispose();
    }
}