using data_access.redis;

namespace org_service
{
    public record Organisation : IRedisDbEntity
    {
        public string PartitionKey => string.Join('~', [Id, Name]);

        public string Id { get; set; } = Guid.NewGuid().ToString();

        public required string Name { get; set; }
        public string? Description { get; set; }
        public required string Email { get; set; }
        public required string Phone { get; set; }
        public required string Address { get; set; }
        public string? BillingAddress { get; set; }
        public IRedisDbEntity[] Users { get; set; }
        public IRedisDbEntity[] Members { get; set; }
    }
}
