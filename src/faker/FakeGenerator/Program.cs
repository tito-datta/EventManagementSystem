using Bogus;
using data_access.redis;
using data_access.redis.database;
using FakeGenerator;
using models;
using Redis.OM;
using StackExchange.Redis;

internal class Program
{
    private static void Main(string[] args)
    {
        var connString = "redis://localhost:6379";

        OrganisationFaker orgFaker = new();
        var organisations = orgFaker.GenerateBetween(1, 5);

        var connectionProvider = new RedisConnectionProvider(connString);
        RedisDbService<Organisation> dbSvc = new(connectionProvider);
        connectionProvider.Connection.CreateIndex(typeof(Organisation));
        
        organisations.ForEach(async o => 
        {
            Console.WriteLine($"Creating {o.Name} fake organisation...");
            await dbSvc.CreateAsync(o); 
        });

        Console.WriteLine($"Generated {organisations.Count()} fake items...");
        Console.WriteLine("Done...");
        Console.ReadLine();
    }
}