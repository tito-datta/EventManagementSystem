using access;

namespace user_access
{
    public class UserEntity : ICosmosEntity
    {
        public string Id { get; set; }

        public string PartitionKey => Id+LastName;

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public long PhoneNumber { get; set; }

        public string CosmosEntityName => "User";   
    }
}