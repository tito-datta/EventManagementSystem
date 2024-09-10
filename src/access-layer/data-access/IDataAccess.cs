using DataAccess.Redis;
using System.Collections.Immutable;
using System.Linq.Expressions;

namespace DataAccess
{
    public interface IDataAccess<T> where T : IRedisEntity
    {
        Task<T> GetAsync(string id);
        Task CreateAsync(T item);
        Task DeleteAsync(T item);
        Task<T[]> QueryAsync(Func<IQueryable<T>, Task<T[]>> query);
        Task<S[]> QueryAsync<S>(Func<IQueryable<T>, Task<S[]>> query);
        Task<bool> UpdateAsync(T item);
    }
}