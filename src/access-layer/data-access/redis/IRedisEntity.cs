namespace data_access.redis
{
    public interface IRedisEntity
    {
        string Id { get; }
        string PartitionKey { get; }
    }
}