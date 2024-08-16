using data_access.redis;
using Redis.OM.Modeling;
using System.Text.Json.Serialization;

namespace models
{

    public record Member : MemberBase, IRedisDbEntity
    {
        [RedisIdField]
        [Indexed]
        public Ulid Id { get; set; } = Ulid.NewUlid();

        public required string OrganisationId { get; set; }

        [Indexed(CascadeDepth = 1)]
        public MemberBase[] Dependents { get; set; }

        [JsonIgnore]
        public string PartitionKey => OrganisationId;
    }

    public record MemberBase
    {
        [Searchable]
        public required string Name { get; set; }

        [Indexed]
        public required string Email { get; set; }

        [Indexed]
        public required string PhoneNumber { get; set; }

        public string Address { get; set; }
    }
}
