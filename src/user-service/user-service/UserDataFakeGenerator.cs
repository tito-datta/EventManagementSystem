using Bogus;
using data_access.redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace user_service
{
    public class UserDataFakeGenerator
    {
        private readonly UserFaker _userFaker;
        private readonly RedisCacheService<User> _redisCacheService;

        public UserDataFakeGenerator(RedisCacheService<User> redisCacheService)
        {
            _userFaker = new UserFaker();
            _redisCacheService = redisCacheService;
        }

        public async Task GenerateAndStoreTestData(int count)
        {
            var users = _userFaker.Generate(count);
            foreach (var user in users)
            {
                await _redisCacheService.CreateAsync(user);
            }
        }
    }

    public class UserFaker : Faker<User>
    {
        public UserFaker()
        {
            RuleFor(u => u.Id, f => Guid.NewGuid().ToString());
            RuleFor(u => u.OrganisationId, f => f.Name.FirstName());
            RuleFor(u => u.Name, f => f.Name.LastName());
            RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.Name,u.OrganisationId));
        }
    }
}
