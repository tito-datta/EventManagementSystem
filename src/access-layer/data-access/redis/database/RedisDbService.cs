using Microsoft.Extensions.Logging;
using Redis.OM.Contracts;
using Redis.OM.Searching;

namespace DataAccess.Redis.Database;

public class RedisDbService<T> : IDataAccess<T> where T : IRedisDbEntity
{
    private readonly IRedisConnectionProvider _connectionProvider;
    private readonly IRedisCollection<T> _collection;
    private readonly ILogger<T> _logger;

    public RedisDbService(
        IRedisConnectionProvider connectionProvider,        
        ILogger<T> logger,
        IRedisCollection<T> collection = null)
    {
        _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
        _collection = collection ?? _connectionProvider.RedisCollection<T>();
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<T> GetAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentException("Id cannot be null or empty", nameof(id));
        }

        _logger.LogDebug("Fetching item with key: {Id}", id);
        try
        {
            var result = await _collection.FirstOrDefaultAsync(a => a.Id.ToString() == id);
            if (result is null)
            {
                _logger.LogDebug("No item found with key: {Id}", id);
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching item with key: {Id}", id);
            throw;
        }
    }

    public async Task CreateAsync(T item)
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        _logger.LogDebug("Creating item with key: {Id}", item.Id);
        try
        {
            var exists = await _collection.AnyAsync(a => a.Id == item.Id);
            if (exists)
            {
                throw new InvalidOperationException($"Item with id {item.Id} already exists");
            }

            await _collection.InsertAsync(item);
            await _collection.SaveAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating item with key: {Id}", item.Id);
            throw;
        }
    }

    public async Task DeleteAsync(T item)
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        _logger.LogDebug("Deleting item with key: {Id}", item.Id);
        try
        {
            var existingItem = await GetAsync(item.Id.ToString());
            if (existingItem is null)
            {
                throw new InvalidOperationException($"Item with id {item.Id} does not exist");
            }

            await _collection.DeleteAsync(item);
            await _collection.SaveAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting item with key: {Id}", item.Id);
            throw;
        }
    }

    public async Task<T[]> QueryAsync(Func<IQueryable<T>, Task<T[]>> query)
    {
        try
        {
            return await query(_collection.AsQueryable());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing query");
            throw;
        }
    }

    public async Task<S[]> QueryAsync<S>(Func<IQueryable<T>, Task<S[]>> query)
    {
        try
        {
            return await query(_collection.AsQueryable());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing query");
            throw;
        }
    }

    public async Task<bool> UpdateAsync(T item)
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        _logger.LogDebug("Updating item with key: {Id}", item.Id);
        try
        {
            var existingItem = await GetAsync(item.Id.ToString());
            if (existingItem is null)
            {
                _logger.LogWarning("Failed to update item with key: {Id}. Item not found.", item.Id);
                return false;
            }

            await _collection.UpdateAsync(item);
            await _collection.SaveAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating item with key: {Id}", item.Id);
            return false;
        }
    }
}