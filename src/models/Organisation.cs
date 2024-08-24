using data_access.redis;
using Redis.OM.Modeling;

namespace models;

[Document(StorageType = StorageType.Json, Prefixes = ["Organisation"])]
public record Organisation : IRedisDbEntity
{
    [RedisIdField]
    [Indexed]
    public Ulid Id { get; set; } = Ulid.NewUlid();
    
    [Searchable]
    public required string Name { get; set; }
    
    public string? Description { get; set; }
    
    [Indexed]
    public required string Email { get; set; }
    
    public required string Phone { get; set; }
    
    public required string Address { get; set; }
    
    public string? BillingAddress { get; set; }

    [Indexed(CascadeDepth = 1)]
    public User[] Users { get; set; } = [];

    [Indexed(CascadeDepth = 2)]
    public Member[] Members { get; set; } = [];


}

