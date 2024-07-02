namespace access
{
    public interface ICosmosEntity
    {
        string Id { get; }
        string PartitionKey { get; }
        string CosmosEntityName { get; }
    }
}