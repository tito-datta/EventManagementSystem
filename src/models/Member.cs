using DataAccess.Redis;
using Redis.OM.Modeling;

namespace models
{
    [Document(StorageType = StorageType.Json, Prefixes = ["Member"])]
    public record Member : MemberBase, IRedisDbEntity
    {
        [RedisIdField]
        [Indexed]
        public Ulid Id { get; set; } = Ulid.NewUlid();

        [Indexed]
        public required string OrganisationId { get; set; }

        [Indexed(CascadeDepth = 1)]
        public MemberBase[] Dependents { get; set; } = [];
    }

    [Document(StorageType = StorageType.Json, Prefixes = ["MemberBase"])]
    public record MemberBase
    {
        [Searchable]
        public required string Name { get; set; }

        [Indexed]
        public required string Email { get; set; }

        [Indexed, Searchable]
        public required string PhoneNumber { get; set; }

        public GeoLoc Address { get; set; }
    }
}
