using StackExchange.Redis;
using System.Text.Json;

namespace data_access.redis
{
    public interface IRedisEntity
    {
        public string Id { get; }        
    }

    public interface IRedisDbEntity : IRedisEntity
    {
        public string PartitionKey { get; }
    }

    public interface IRedisCacheEntity : IRedisEntity
    {
        public object Value { get; set; }
        private string _value => JsonSerializer.Serialize(Value);
        internal HashEntry CacheEntry => new(new(Id), new(_value));
    }
}