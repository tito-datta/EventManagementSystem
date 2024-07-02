using access;
using Microsoft.Azure.Cosmos;

public abstract class CosmosDb<T> where T : ICosmosEntity
{
    private readonly CosmosClient _client;
    private readonly Container _container;

    public CosmosDb(string connectionString, string databaseName, string containerName)
    {
        _client = new CosmosClient(connectionString);
        _container = _client.GetContainer(databaseName, containerName);
    }  

    public async Task<CosmosResult<T>> CreateItemAsync(T item)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));

        var response = await _container.CreateItemAsync(item, new PartitionKey(item.PartitionKey));

        if (response is null || response.StatusCode is System.Net.HttpStatusCode.Created) 
        {
            return new CosmosResult<T>(Errors: [string.Format("Failed to create {0} object in db for {1}, Status Code: {2}.",nameof(T), item.Id, response?.StatusCode)]);
        }

        return new(Item: response.Resource);
    }

    public async Task<CosmosResult<T>> ReadItemAsync(string id, string partitionKey)
    {
        if (id is null) throw new ArgumentNullException(nameof(id));
        if (partitionKey is null) throw new ArgumentNullException(nameof(partitionKey));

        var response = await _container.ReadItemAsync<T>(id, new PartitionKey(partitionKey));

        if(response is null || response.StatusCode is not System.Net.HttpStatusCode.OK)
        {
            return new(Errors: [string.Format("Failed to read {0} object with {1}, Status Code: {2}.", nameof(T), id, response?.StatusCode)]);
        }

        return new(Item: response.Resource);
    }

    public async Task<CosmosResult<T>> UpdateItemAsync(T item)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));

        var response = await _container.UpsertItemAsync(item, new PartitionKey(item.PartitionKey));

        if(response is null || response.StatusCode is not System.Net.HttpStatusCode.OK || response.StatusCode is not System.Net.HttpStatusCode.Created)
        {
            return new(Errors: [string.Format("Failed to update {0} object with {1}, Status Code: {2}.", nameof(T), item.Id, response?.StatusCode)]);
        }

        return new(Item: response.Resource);
    }

    public async Task DeleteItemAsync(string id, string partitionKey) => await _container.DeleteItemAsync<T>(id, new PartitionKey(partitionKey));
}