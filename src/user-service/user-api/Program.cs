using data_access.cosmos;
using data_access.redis;
using data_access.redis.database;
using Microsoft.Azure.Cosmos;
using Microsoft.OpenApi.Models;
using user_api;
using user_service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "User Services", Version = "v1" });
});

// Register services
var userdb = builder.Configuration.GetSection("UserDb");
// Register Db
//builder.Services.AddSingleton(new CosmosDbService<user_service.User>(userdb.GetSection("ConnectionString").Value!,
//                                                                     userdb.GetSection("DatabaseName").Value!,
//                                                                     userdb.GetSection("ContainerName").Value!));
builder.Services.AddSingleton(new RedisDbService<user_service.User>(builder.Configuration.GetSection("Redis").Value!, "User-Service"));
builder.Services.AddSingleton(s => new UserDataFakeGenerator(s.GetRequiredService<RedisDbService<user_service.User>>()));
builder.Services.AddScoped(s => new UserService(s.GetRequiredService<RedisDbService<user_service.User>>()));

// Register healthchecks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "User Services v1"));
}

app.UseHttpsRedirection();

app.MapUserEndpoints();

app.MapPost("users-gen-fake-data", async (int count, UserDataFakeGenerator generator) =>
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

app.MapHealthChecks("/health");

app.Run();