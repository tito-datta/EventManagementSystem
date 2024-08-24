using data_access.redis;
using Redis.OM.Modeling;

namespace models;

[Document(StorageType = StorageType.Json, Prefixes = ["User"])]
public class User : IRedisDbEntity
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

    public required string Phone { get; set; }

    public int Type { get; set; } = 1;
}

//public record UserCacheDto : IRedisCacheEntity
//{
//    public UserCacheDto(User user)
//    {
//        _user = user;
//    }
//    public object Value => _user;

//    public string Id => string.Join('~', [_user.OrganisationId, _user.Id]);

//    private readonly User _user;
//}
