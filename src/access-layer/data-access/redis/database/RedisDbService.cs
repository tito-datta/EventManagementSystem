using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Text.Json;
using Redis.OM;
using Redis.OM.Contracts;
using Redis.OM.Searching;
using StackExchange.Redis;

namespace data_access.redis.database
{
    public class RedisDbService<T> : IDataAccess<T> where T : IRedisDbEntity
    {
        private readonly IRedisConnectionProvider _connectionProvider;
        private readonly IRedisCollection<T> _collection;

        public RedisDbService(string connectionString, string prefix)
        {
            var options = ConfigurationOptions.Parse(connectionString);
            options.SyncTimeout = 15000; // 15 seconds
            options.ConnectTimeout = 15000; // 15 seconds

            _connectionProvider = new RedisConnectionProvider(options);
            _connectionProvider.Connection.CreateIndex(typeof(T));
            _collection = new RedisCollection<T>(_connectionProvider.Connection);
        }

        public async Task<T> GetAsync(string id, string partKey)
        {
            var result = await _collection.SingleAsync(a => a.Id.ToString() == id);
            if(result != null)
            {
                return result;
            }

            return default;
        }

        public async Task CreateAsync(T item)
        {
            if (await _collection.AnyAsync(a => a.Id == item.Id))
            {
                await UpdateAsync(item);
            }
            else
            {
                await _collection.InsertAsync(item);
            }

            await _collection.SaveAsync();
        }

        public async Task DeleteAsync(T item) => await _collection.DeleteAsync(item);

        public async Task<T[]> QueryAsync(Func<ImmutableArray<T>, Task<T[]>> query) => await query(_collection.ToImmutableArray());

        public async Task<S[]> QueryAsync<S>(Func<ImmutableArray<T>, Task<S[]>> query) => await query(_collection.ToImmutableArray());

        public async Task<bool> UpdateAsync(T item)
        {
            try
            {
                await _collection.UpdateAsync(item);
                await _collection.SaveAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}