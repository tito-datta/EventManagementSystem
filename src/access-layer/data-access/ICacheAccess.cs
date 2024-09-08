using DataAccess.Redis;

namespace DataAccess
{
    public interface ICacheAccess<T> where T : IRedisEntity
    {
        Task<T> GetAsync(string hashKey, string fieldId);
        Task<T[]> GetManyAsync(string hashKey);
        Task SetAsync(string hashKey, T entity);
        Task SetManyAsync(string hashKey, T[] entites);
        Task<bool> DeleteAsync(string hashKey, string fieldId);
        Task<bool> ExistsAsync(string hashKey, string fieldId);
    }
}