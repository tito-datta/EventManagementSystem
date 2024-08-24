using System.Collections.Immutable;
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

        public RedisDbService(IRedisConnectionProvider connPro)
        {
            _connectionProvider = connPro;

            _collection = new RedisCollection<T>(_connectionProvider.Connection, true, chunkSize: 300);
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
            if(item == null) throw new ArgumentNullException(nameof(item));

            var isPresent = _collection.Where(a => a.Id == item.Id) != null;

            if (!isPresent)
            {
                await _collection.InsertAsync(item);
            }
            else
            {
                await _collection.UpdateAsync(item);
            }

            await _collection.SaveAsync();
        }

        public async Task DeleteAsync(T item)
        {
            if(item is null) throw new ArgumentNullException(nameof(item));

            await _collection.DeleteAsync(item);
        }

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