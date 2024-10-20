using Bogus;
using DataAccess.Redis;
using DataAccess.Redis.Database;
using FakeGenerator;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using models;
using Moq;
using Redis.OM;
using StackExchange.Redis;

internal class Program
{
    private static void Main(string[] args)
    {
        var connString = "redis://localhost:6379";
        var logFactory = new NullLoggerFactory();

        OrganisationFakerTwoOrganisationsHaveSimilarMemeberNames orgFaker = new();
        var organisations = orgFaker.GenerateBetween(1000, 2150);

        var connectionProvider = new RedisConnectionProvider(connString);
        Lazy<RedisDbService<Organisation>> dbSvc = new(() => new(connectionProvider, logFactory.CreateLogger<Organisation>(), null)); // passing null since we do not yet fetch the collection here
        // RedisDbService<Organisation> dbSvc = new(connectionProvider, logFactory.CreateLogger<Organisation>(), null); 
        connectionProvider.Connection.CreateIndex(typeof(Organisation));

        organisations.ForEach(async o =>
        {
            Console.WriteLine($"Creating {o.Name} fake organisation...");
            await dbSvc.Value.CreateAsync(o);
        });

        Console.WriteLine($"Generated {organisations.Count()} fake items...");
        Console.WriteLine("Done...");
        Console.ReadLine();
    }
}