using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace data_access.redis.cache
{
    public class RedisCacheService<T> : ICacheAccess<T>, IDisposable where T : class, IRedisCacheEntity
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _db;
        private readonly ILogger<T> _logger;

        public RedisCacheService(string connectionString, ILogger<T> logger)
        {
            _redis = ConnectionMultiplexer.Connect(connectionString);
            _db = _redis.GetDatabase();
            _logger = logger;
        }



        public async Task<T> GetAsync(string hashKey, string fieldId)
        {
            try
            {
                var value = await _db.HashGetAsync(hashKey, fieldId);
                if (value.IsNull){
                    _logger.LogDebug(string.Format("No match found for key: {0}", hashKey));
                    return default;
                }

                _logger.LogDebug(string.Format("Match found for key: {0}", hashKey));
                return value as T;
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("Fetch from cache for key: {0} failed due to \n{1}\nat {2}.", hashKey, ex.Message, ex.StackTrace));
                return default;
            }
        }

        public async Task SetAsync(string hashKey, T entity)
        {
            if(entity == null) throw new ArgumentNullException(nameof(entity));
            try
            {
                _logger.LogDebug(string.Format("Saved key: {0} to cache", hashKey));
                await _db.HashSetAsync(hashKey, [new(entity.Id.ToString(), new(entity.ValueStr))], CommandFlags.DemandMaster);
            }
            catch (Exception ex) 
            {
                _logger.LogError(string.Format("Failed to save key: {0} to cache due to \n{1}\nat {2}", hashKey, ex.Message, ex.StackTrace));
            }
        }

        public async Task<bool> DeleteAsync(string hashKey, string fieldId)
        {
            try
            {
                
                _logger.LogDebug(string.Format("Deleting item with key: {0} {1} from cache.", hashKey, fieldId));
                var isSuccess = await _db.HashDeleteAsync(hashKey, fieldId);
                if (isSuccess)
                {
                    _logger.LogDebug(string.Format("Deleted item with key: {0} {1} from cache.", hashKey, fieldId));
                }
                _logger.LogDebug(string.Format("No match found for key: {0} {1} from cache.", hashKey, fieldId));

                return isSuccess;
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("Failed to delete key: {0} {1} to cache due to \n{2}\nat {3}", hashKey, fieldId, ex.Message, ex.StackTrace));
                return false;
            }
        }

        public async Task<bool> ExistsAsync(string hashKey, string fieldId) => await _db.HashExistsAsync(hashKey, fieldId);

        public void Dispose() => _redis.Dispose();

        public async Task<T[]> GetManyAsync(string hashKey)
        {
            try
            {
                var values = await _db.HashGetAllAsync(hashKey, CommandFlags.PreferReplica);

                if (values is null || values.Length == 0)
                {
                    _logger.LogDebug(string.Format("Fetched 0 results from cache for\n key: {0}.", hashKey));
                    return [];
                }

                _logger.LogDebug(string.Format("Fetched {0} results from cache for\n key: {1}.", values.Length, hashKey));
                return values.Select(v => v.Value as T).ToArray()!;
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("Fetch from cache for key: {0} failed due to \n{1}\nat {2}.", hashKey, ex.Message, ex.StackTrace));
                return [];
            }
        }

        public async Task SetManyAsync(string hashKey, T[] entites)
        {
            try
            {
                await _db.HashSetAsync(hashKey, entites.Select(e => e.CacheEntry).ToArray(), CommandFlags.DemandMaster);
                _logger.LogDebug(string.Format("Saved key: {0} to cache", hashKey));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("Save to cache for key: {0} failed due to \n{1}\nat {2}.", hashKey, ex.Message, ex.StackTrace));
            }
        }
    }
}