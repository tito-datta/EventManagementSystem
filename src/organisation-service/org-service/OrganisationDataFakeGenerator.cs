using data_access.redis.database;
using Bogus;
using Bogus.Extensions.UnitedStates;
using data_access.redis;
using Microsoft.Azure.Cosmos;

namespace org_service
{
    public class OrganisationDataFakeGenerator
    {
        private readonly OrganisationFaker _organisationFaker;
        private readonly RedisDbService<Organisation> _redisCacheService;
        public OrganisationDataFakeGenerator(RedisDbService<Organisation> redisDbService)
        {
            _organisationFaker = new();
            _redisCacheService = redisDbService;
        }        

        public async Task GenerateAndStoreTestData(int count)
        {
            var organisations = _organisationFaker.Generate(count);
            organisations.ForEach(async org => await _redisCacheService.CreateAsync(org));
        }
    }

    public class OrganisationFaker : Faker<Organisation>
    {
        public OrganisationFaker()
        {
            RuleFor(o => o.Id, f => f.Company.Ein());
            RuleFor(o => o.Name, f => f.Company.CompanyName());
            RuleFor(o => o.Email, f => f.Internet.ExampleEmail());
            RuleFor(o => o.Address, f => f.Address.FullAddress());
            RuleFor(o => o.BillingAddress, f => {
                if (f.Random.Bool())
                {
                    f.Address.SecondaryAddress();
                }
                return string.Empty;
            });
            RuleFor(o => o.Description, f => f.Company.Bs());
            RuleFor(o => o.Members, (f, o) => new MemberFaker(o.Name).GenerateBetween(4,10).ToArray());
            RuleFor(o => o.Users, (f, o) => new UserFaker(o.Name).GenerateBetween(1, 4).ToArray());
        }
    }

    internal class Member : IRedisDbEntity
    {
        public string PartitionKey => Organisation;

        public required string Organisation { get; set; }
        public required string Name { get; set; }

        public string Id => System.Guid.NewGuid().ToString();
    }

    internal class MemberFaker : Faker<Member>
    {
        public MemberFaker(string organisation)
        {
            RuleFor(m => m.Organisation, () => organisation);
            RuleFor(m => m.Name, f => f.Name.FullName());
        }
    }

    internal class User : IRedisDbEntity
    {
        public string PartitionKey => Organisation;

        public required string Organisation { get; set; }
        public required string Name { get; set; }

        public string Id => System.Guid.NewGuid().ToString();
    }

    internal class UserFaker : Faker<Member>
    {
        public UserFaker(string organisation)
        {
            RuleFor(m => m.Organisation, () => organisation);
            RuleFor(m => m.Name, f => f.Name.FullName());
        }
    }
}
