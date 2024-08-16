using StackExchange.Redis;
using System.Text.Json;

namespace data_access.redis
{
    public interface IRedisEntity
    {
        public Ulid Id { get; }        
    }

    public interface IRedisDbEntity : IRedisEntity
    {
        public string PartitionKey { get; }
    }

    public interface IRedisCacheEntity : IRedisEntity
    {
        public object Value { get; }
        internal string ValueStr => JsonSerializer.Serialize(Value);
        private string _value => JsonSerializer.Serialize(Value);
        internal HashEntry CacheEntry => new(new(Id.ToString()), new(_value));
    }
}