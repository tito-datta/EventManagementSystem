using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace data_access.redis.cache
{
    internal class RedisCacheService<T> : IDataAccess<T>, IDisposable where T : IRedisEntity, new()
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _db;
        private readonly TimeSpan _defaultTtl;

        public RedisCacheService(string connectionString, TimeSpan? defaultTtl = null)
        {
            _redis = ConnectionMultiplexer.Connect(connectionString);
            _db = _redis.GetDatabase();
            _defaultTtl = defaultTtl ?? TimeSpan.FromMinutes(3); // Default TTL of 3 minutes if not specified
        }

        public async Task<T> GetAsync(string id, string partKey)
        {
            var key = $"{typeof(T).Name}:{partKey}:{id}";
            var value = await _db.StringGetAsync(key);
            return value.IsNull ? default : JsonSerializer.Deserialize<T>(value);
        }

        public async Task CreateAsync(T item)
        {
            await UpdateAsync(item);
        }

        public async Task DeleteAsync(T item)
        {
            var id = GetIdFromItem(item);
            var partKey = GetPartitionKeyFromItem(item);
            var key = $"{typeof(T).Name}:{partKey}:{id}";
            await _db.KeyDeleteAsync(key);
        }

        public async Task<T[]> GetAllAsync()
        {
            var server = _redis.GetServer(_redis.GetEndPoints()[0]);
            var keys = server.Keys(pattern: $"{typeof(T).Name}:*");
            var tasks = new List<Task<T>>();

            foreach (var key in keys)
            {
                tasks.Add(Task.Run(async () =>
                {
                    var value = await _db.StringGetAsync(key);
                    return JsonSerializer.Deserialize<T>(value);
                }));
            }

            return await Task.WhenAll(tasks);
        }

        public async Task UpdateAsync(T item)
        {
            var id = GetIdFromItem(item);
            var partKey = GetPartitionKeyFromItem(item);
            var key = $"{typeof(T).Name}:{partKey}:{id}";
            var value = JsonSerializer.Serialize(item);
            await _db.StringSetAsync(key, value, _defaultTtl);
        }

        private string GetIdFromItem(T item) => item.Id;

        private string GetPartitionKeyFromItem(T item) => item.PartitionKey;

        public void Dispose()
        {
            _redis.Dispose();
        }
    }
}
