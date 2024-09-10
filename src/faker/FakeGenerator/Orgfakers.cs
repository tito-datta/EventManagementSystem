using Bogus;
using models;
using Redis.OM.Modeling;
using System.Security.Cryptography;

namespace FakeGenerator
{
    public class OrganisationFaker : Faker<Organisation>
    {
        public OrganisationFaker()
        {
            RuleFor(o => o.Id, () => Ulid.NewUlid());
            RuleFor(o => o.Name, f => f.Company.CompanyName());
            RuleFor(o => o.Email, (f, o) => f.Internet.ExampleEmail(o.Name, o.Id.ToString()));
            RuleFor(o => o.Address, f => f.Address.FullAddress());
            RuleFor(o => o.Phone, (f) => f.Phone.PhoneNumber("(91) ##### #####"));
            RuleFor(o => o.BillingAddress, f => {
                if (f.Random.Bool())
                {
                    f.Address.SecondaryAddress();
                }
                return string.Empty;
            });
            RuleFor(o => o.Description, f => f.Company.Bs());
            RuleFor(o => o.Members, (f, o) => new MemberFaker(o.Id.ToString(), o.Name).GenerateBetween(4, 10).ToArray());
            RuleFor(o => o.Users, (f, o) => new UserFaker(o.Id.ToString(), o.Name).GenerateBetween(1, 4).ToArray());
        }
    }

    public class MemberFaker : Faker<Member>
    {
        public MemberFaker(string orgId, string organisation)
        {
            RuleFor(m => m.Id, f => Ulid.NewUlid());
            RuleFor(m => m.OrganisationId, () => orgId);
            RuleFor(m => m.Name, f => f.Name.FullName());
            RuleFor(m => m.Email, (f, m) => f.Internet.Email(m.Name.Split(" ")[0], m.Name.Split(" ")[1]));
            RuleFor(m => m.PhoneNumber, f => f.Phone.PhoneNumber("(91) ##### #####"));
            RuleFor(m => m.Address, f => new GeoLoc(f.Address.Longitude(), f.Address.Latitude()));
            RuleFor(m => m.Dependents,
                    (f, m) => new MemberBaseFaker(m.Name.Split(" ")[1], m.Address).GenerateBetween<MemberBase>(1, 5).ToArray());
        }
    }

    public class MemberBaseFaker : Faker<MemberBase>
    {
        public MemberBaseFaker(string lastName, GeoLoc address)
        {
            RuleFor(mb => mb.Name, f => f.Name.FirstName() + " " + lastName);
            RuleFor(mb => mb.Email, (f, mb) => f.Internet.Email(mb.Name));
            RuleFor(mb => mb.Address, () => address);
            RuleFor(mb => mb.PhoneNumber, f => f.Phone.PhoneNumber("(91) ##### #####"));
        }
    }

    public class UserFaker : Faker<User>
    {
        public UserFaker(string orgId, string orgName)
        {
            RuleFor(u => u.Id, f => Ulid.NewUlid());
            RuleFor(u => u.OrganisationId, () => orgId);
            RuleFor(u => u.Name, f => f.Name.FullName());
            RuleFor(u => u.Phone, f => f.Phone.PhoneNumber("(91) ##### #####"));
            RuleFor(u => u.Type, f => f.Random.Bool() ? 1 : 0);
            RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.Name.Split(" ")[0], u.Name.Split(" ")[1]));
        }
    }

    public class OrganisationFakerTwoOrganisationsHaveSimilarMemeberNames : Faker<Organisation>
    {
        public OrganisationFakerTwoOrganisationsHaveSimilarMemeberNames()
        {
            RuleFor(o => o.Id, () => Ulid.NewUlid());
            RuleFor(o => o.Name, f => f.Company.CompanyName());
            RuleFor(o => o.Email, (f, o) => f.Internet.ExampleEmail(o.Name, o.Id.ToString()));
            RuleFor(o => o.Address, f => f.Address.FullAddress());
            RuleFor(o => o.Phone, (f) => f.Phone.PhoneNumber("(91) ##### #####"));
            RuleFor(o => o.BillingAddress, f => {
                if (f.Random.Bool())
                {
                    f.Address.SecondaryAddress();
                }
                return string.Empty;
            });
            RuleFor(o => o.Description, f => f.Company.Bs());
            RuleFor(o => o.Members,
                    (f, o) => f.Random.Int(1, 3) == 3 
                    ? [.. new MemberFakerSpecial(o.Id.ToString(), "Suraj Naik").GenerateBetween(1, 1), .. new MemberFaker(o.Id.ToString(), o.Name).GenerateBetween(4, 10)]
                    : [.. new MemberFaker(o.Id.ToString(), o.Name).GenerateBetween(4, 10)]);
            RuleFor(o => o.Users, (f, o) => 
                    f.Random.Int(1, 3) == 3 
                    ? [.. new UserFakerSpecial(o.Id.ToString(), "Bad Bobbin").GenerateBetween(1, 1), .. new UserFaker(o.Id.ToString(), o.Name).GenerateBetween(1, 4)] 
                    : [.. new UserFaker(o.Id.ToString(), o.Name).GenerateBetween(1, 4)]);            
        }

        private class MemberFakerSpecial : Faker<Member>
        {
            public MemberFakerSpecial(string orgId, string name)
            {
                RuleFor(m => m.Id, f => Ulid.NewUlid());
                RuleFor(m => m.OrganisationId, () => orgId);
                RuleFor(m => m.Name, f => name);
                RuleFor(m => m.Email, (f, m) => f.Internet.Email(m.Name.Split(" ")[0], m.Name.Split(" ")[1]));
                RuleFor(m => m.PhoneNumber, f => f.Phone.PhoneNumber("(91) ##### #####"));
                RuleFor(m => m.Address, f => new GeoLoc(f.Address.Longitude(), f.Address.Latitude()));
                RuleFor(m => m.Dependents,
                        (f, m) => new MemberBaseFaker(m.Name.Split(" ")[1], m.Address).GenerateBetween<MemberBase>(1, 5).ToArray());
            }
        }

        private class UserFakerSpecial : Faker<User>
        {
            public UserFakerSpecial(string orgId, string name)
            {
                RuleFor(u => u.Id, f => Ulid.NewUlid());
                RuleFor(u => u.OrganisationId, () => orgId);
                RuleFor(u => u.Name, f => name);
                RuleFor(u => u.Phone, f => f.Phone.PhoneNumber("(91) ##### #####"));
                RuleFor(u => u.Type, f => f.Random.Bool() ? 1 : 0);
                RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.Name.Split(" ")[0], u.Name.Split(" ")[1]));
            }
        }
    }
}
