using StackExchange.Redis;
using System.Text.Json;
using System;
using Redis.OM.Modeling;

namespace data_access.redis
{
    public interface IRedisEntity
    {
        [RedisIdField]
        public Ulid Id { get; }        
    }

    public interface IRedisDbEntity : IRedisEntity
    {
        // i shall fix it later
        //public string PartitionKey { get; }
    }

    public interface IRedisCacheEntity : IRedisEntity
    {
        public object Value { get; }
        internal string ValueStr => JsonSerializer.Serialize(Value);
        private string _value => JsonSerializer.Serialize(Value);
        internal HashEntry CacheEntry => new(new(Id.ToString()), new(_value));
    }
}