using data_access.redis;
using System.Collections.Immutable;
using System.Linq.Expressions;

namespace data_access
{
    public interface IDataAccess<T> where T : IRedisEntity
    {
        Task<T> GetAsync(string id, string partKey);
        Task CreateAsync(T item);
        Task DeleteAsync(T item);
        Task<T[]> QueryAsync(Func<ImmutableArray<T>, Task<T[]>> query);
        Task<S[]> QueryAsync<S>(Func<ImmutableArray<T>, Task<S[]>> query);
        Task<bool> UpdateAsync(T item);
    }
}