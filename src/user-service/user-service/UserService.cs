using data_access.cosmos;
using System.Text.Json.Serialization;

namespace user_service
{
    // TO-DO: Exception handling lol, this gonna break soon
    public class UserService
    {
        private readonly CosmosDbService<User> _dbSvc;
        public UserService(CosmosDbService<User> dbSvc)
        {
            _dbSvc = dbSvc;
        }

        public async Task<User[]> Get() => _dbSvc.GetAllAsync().Result;
        
        public async Task<User> GetById(string id, string organisationId) => await _dbSvc.GetAsync(id, organisationId);

        public async Task Create(User user) => await _dbSvc.CreateAsync(user);
    }

    public record User : ICosmosEntity
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        public string OrganisationId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        [JsonPropertyName("CosmosEntityName")]
        public string CosmosEntityName { get; init; } = "User";

        public string PartitionKey => OrganisationId;
    }
}
