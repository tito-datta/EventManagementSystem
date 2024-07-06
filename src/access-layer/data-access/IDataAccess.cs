using data_access.cosmos;

namespace data_access
{
    public interface IDataAccess<T> where T : new()
    {
        Task<T> GetAsync(string id, string partKey);
        Task CreateAsync(T item);
        Task DeleteAsync(T item);
        Task<T[]> GetAllAsync();
        Task UpdateAsync(T item);
    }
}