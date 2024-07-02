using Microsoft.Azure.Cosmos;

namespace data_access.cosmos
{
    public class CosmosDbService<T> : IDataAccess<T> where T : ICosmosEntity, new()
    {
        private readonly CosmosClient _client;
        private readonly Database _db;
        private readonly Container _container;

        public CosmosDbService(string connString, string database, string container)
        {
            _client = new(connString);
            _db = _client.GetDatabase(database);
            _container = _db.GetContainer(container);
        }

        public async Task CreateAsync(T item)
        {
            if(item is null) throw new ArgumentNullException(nameof(item));

            var response = await _container.CreateItemAsync(item);
            if(response is null || response.StatusCode is not System.Net.HttpStatusCode.Created)
            {
                throw new Exception(string.Format("Failed to create object {0} from database due to {1}.", item.GetType().Name, response?.StatusCode));
            }            
        }

        public async Task DeleteAsync(T item)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));

            var response = await _container.DeleteItemAsync<T>(item.Id, new(item.PartitionKey));
            if (response is null || response.StatusCode is not System.Net.HttpStatusCode.NoContent)
            {
                throw new Exception(string.Format("Failed to delete object {0} from database due to {1}.", item.GetType().Name, response?.StatusCode));
            }
        }

        public async Task<T[]> GetAllAsync()
        {
            var response = _container.GetItemLinqQueryable<T>(allowSynchronousQueryExecution: true);

            if(response is null || response.Count() == 0)
            {
                throw new Exception("No data found.");
            }

            return response.ToArray();
        }

        public async Task<T> GetAsync(string id, string partKey)
        {
            if (id is null) throw new ArgumentNullException(nameof(id));

            var response = await _container.ReadItemAsync<T>(id, new(partKey));
            if (response is null || response.StatusCode is not System.Net.HttpStatusCode.OK)
            {
                throw new Exception(string.Format("Failed to read object with key {0} & partition key {1} from database due to {1}.", id, partKey, response?.StatusCode));
            }

            return response.Resource;
        }

        public async Task UpdateAsync(T item)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));

            var response = await _container.UpsertItemAsync<T>(item, new(item.PartitionKey));
            if (response is null || response.StatusCode is not System.Net.HttpStatusCode.NoContent)
            {
                throw new Exception(string.Format("Failed to update object {0} in database due to {1}.", item.GetType().Name, response?.StatusCode));
            }
        }
    }
}
