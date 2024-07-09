using data_access.redis.database;
using MebershipService;
using membership_api;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Membership Service", Version = "v1" });
});

// Register services
// Register Db
builder.Services.AddSingleton(new RedisDbService<Member>(builder.Configuration.GetSection("Redis").Value!, "Membership-Service"));
builder.Services.AddSingleton(s => new MembershipDataFakeGenerator(s.GetRequiredService<RedisDbService<Member>>()));
builder.Services.AddScoped(s => new MembershipService(s.GetRequiredService<RedisDbService<Member>>()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Membership Services v1"));
}

app.UseHttpsRedirection();

app.MapMembershipEndpoints();

app.MapPost("members-gen-fake-data", async (int count, MembershipDataFakeGenerator generator) =>
{
    if (count == 0)
    {
        return Results.BadRequest("Count has be at least 1.");
    }

    try
    {
        await generator.GenerateAndStoreTestData(count);
        return Results.Created();
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});


app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
