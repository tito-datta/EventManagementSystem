using Bogus;
using data_access.redis;
using data_access.redis.database;
using FakeGenerator;
using models;

internal class Program
{
    private static void Main(string[] args)
    {
        var connString = "localhost:6378";

        OrganisationFaker orgFaker = new();
        var organisations = orgFaker.GenerateBetween(2, 10);

        RedisDbService<Organisation> dbSvc = new(connString, "Organisation");

        organisations.ForEach(async o => await dbSvc.CreateAsync(o));

        Console.WriteLine("Done...");
        Console.ReadLine();
    }
}