using Bogus;
using data_access.redis.database;
using Microsoft.Azure.Cosmos;

namespace MebershipService
{
    public  class MembershipDataFakeGenerator
    {
        private readonly MemberFaker _mbrFaker;
        private readonly RedisDbService<Member> _redisDbService;

        public MembershipDataFakeGenerator(RedisDbService<Member> redisDbService)
        {
            _mbrFaker = new MemberFaker();
            _redisDbService = redisDbService;
        }

        public async Task GenerateAndStoreTestData(int count)
        {
            var users = _mbrFaker.Generate(count);
            foreach (var user in users)
            {
                await _redisDbService.CreateAsync(user);
            }
        }
    }

    public class MemberFaker : Faker<Member>
    {
        public MemberFaker()
        {
            RuleFor(m => m.Id, f => Guid.NewGuid().ToString());
            RuleFor(m => m.OrganisationId, f => f.Company.CompanyName());
            RuleFor(m => m.Name, f => f.Name.FullName());
            RuleFor(m => m.Email, (f, m) => f.Internet.Email(m.Name));
            RuleFor(m => m.PhoneNumber, f => f.Phone.PhoneNumber());
            RuleFor(m => m.Address, f => f.Address.FullAddress());
            RuleFor(m => m.Dependents,
                    (f, m) => new MemberBaseFaker(m.Name.Split(" ")[1], m.Address).GenerateBetween<MemberBase>(1, 5).ToArray());
        }             
    }

    public class MemberBaseFaker : Faker<MemberBase>
    {
        public MemberBaseFaker(string lastName, string address)
        {
            RuleFor(mb => mb.Name, f => f.Name.FirstName() + " " + lastName);
            RuleFor(mb => mb.Email, (f, mb) => f.Internet.Email(mb.Name));
            RuleFor(mb => mb.Address, () => address);
            RuleFor(mb => mb.PhoneNumber, f => f.Phone.PhoneNumber());
        }
    }
}
