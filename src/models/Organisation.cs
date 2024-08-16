using data_access.redis;
using Redis.OM.Modeling;
using System.Text.Json.Serialization;

namespace models;

[Document(StorageType = StorageType.Hash, Prefixes = ["Organisation"])]
public record Organisation : IRedisDbEntity
{
    [JsonIgnore]
    public string PartitionKey => string.Join('~', [Id.ToString(), Name]);

    [RedisIdField]
    [Indexed]
    public Ulid Id { get; set; } = Ulid.NewUlid();

    [Searchable]
    [JsonRequired]
    public required string Name { get; set; }

    public string? Description { get; set; }

    public required string Email { get; set; }

    public required string Phone { get; set; }

    public required string Address { get; set; }

    public string? BillingAddress { get; set; }

    [Indexed(CascadeDepth = 1)]
    public User[] Users { get; set; } = [default];

    [Indexed(CascadeDepth = 1)]
    public Member[] Members { get; set; } = [default];
}