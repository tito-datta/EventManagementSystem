using data_access.redis;
using Redis.OM.Modeling;
using System.Text.Json.Serialization;

namespace models;

[Document(StorageType = StorageType.Hash, Prefixes = ["User"])]
public record User : IRedisDbEntity
{
    [RedisIdField]
    [Indexed]
    public Ulid Id { get; set; } = Ulid.NewUlid();

    [Indexed]
    public required string OrganisationId { get; set; }

    [Searchable]
    public required string Name { get; set; }

    [Indexed]
    public required string Email { get; set; }

    [Indexed]
    public required string Phone { get; set; }

    public int Type { get; set; } = 1;

    [JsonIgnore]
    public string PartitionKey => OrganisationId;
}

public record UserCacheDto : IRedisCacheEntity
{
    public UserCacheDto(User user)
    {
        _user = user;
    }
    public object Value => _user;

    public Ulid Id => Ulid.Parse(string.Join('~', [_user.OrganisationId, _user.Id]));

    private readonly User _user;
}
