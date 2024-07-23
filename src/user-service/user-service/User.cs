using data_access.cosmos;
using data_access.redis;

namespace user_service
{
    public record User : 
        //ICosmosEntity,
        IRedisDbEntity
    {
        //public string id { get; set; } = Guid.NewGuid().ToString();
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string OrganisationId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string CosmosEntityName { get; } = "User";

        public string PartitionKey => OrganisationId;
    }
}
