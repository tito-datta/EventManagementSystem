using Azure.Core.Serialization;
using data_access.redis;

namespace data_access
{
    public interface ICacheAccess<T> where T : IRedisEntity, new()
    {
        Task<T> GetAsync(string hashKey, string fieldId);
        Task SetAsync(T entity);
        Task<bool> DeleteAsync(string hashKey, string fieldId);
        Task<bool> ExistsAsync(string hashKey, string fieldId);
    }
}