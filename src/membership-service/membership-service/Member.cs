using data_access.redis;

namespace MebershipService
{
    public class Member : MemberBase, IRedisEntity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string OrganisationId { get; set; }

        public MemberBase[] Dependents { get; set; }

        public string PartitionKey => OrganisationId;
    }

    public class MemberBase
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }
    }
}
