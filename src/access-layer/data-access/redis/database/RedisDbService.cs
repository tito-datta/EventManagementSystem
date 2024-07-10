using System.Text.Json;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using StackExchange.Redis;

namespace data_access.redis.database
{
    public class RedisDbService<T> : IDataAccess<T>, IDisposable where T : IRedisEntity, new()
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _db;
        private readonly string _prefix;
        private readonly JsonCommands _json;

        public RedisDbService(string connectionString, string prefix)
        {
            var options = ConfigurationOptions.Parse(connectionString);
            options.AbortOnConnectFail = false;
            options.AsyncTimeout = 10000; // 10 seconds

            _redis = ConnectionMultiplexer.Connect(options);
            _db = _redis.GetDatabase();
            _prefix = prefix;
            _json = _db.JSON();
        }

        private string GetKey(string id) => $"{_prefix}:{id}";

        public async Task<T> GetAsync(string id, string partKey)
        {
            var result = await _json.GetAsync(GetKey(id));
            if(result != null && result != default)
            {
                return JsonSerializer.Deserialize<T>(result.ToString());
            }

            return default;
        }

        public async Task CreateAsync(T item)
        {
            await _json.SetAsync(GetKey(item.Id), "$", JsonSerializer.Serialize(item));
        }

        public async Task DeleteAsync(T item)
        {
            await _db.KeyDeleteAsync(GetKey(item.Id));
        }

        public async Task<T[]> GetAllAsync()
        {
            var server = _redis.GetServer(_redis.GetEndPoints().First());
            var results = new List<T>();

            await foreach (var key in server.KeysAsync(pattern: $"{_prefix}:*"))
            {
                var result = await _json.GetAsync(key);
                if (result != null)
                {
                    var item = JsonSerializer.Deserialize<T>(result.ToString());
                    if (item != null)
                    {
                        results.Add(item);
                    }
                }
            }

            return results.ToArray();
        }

        public async Task UpdateAsync(T item) => await CreateAsync(item);

        public void Dispose()
        {
            _redis.Dispose();
        }
    }
}