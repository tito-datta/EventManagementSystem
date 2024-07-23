using data_access.redis.database;
using Microsoft.OpenApi.Models;
using org_api;
using org_service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Organisation Services", Version = "v1" });
});

// Register services
// Register Db
builder.Services.AddSingleton(new RedisDbService<Organisation>(builder.Configuration.GetSection("Redis").Value!, "Organisation-Service"));
builder.Services.AddSingleton(s => new OrganisationDataFakeGenerator(s.GetRequiredService<RedisDbService<Organisation>>()));
builder.Services.AddScoped(s => new OrganisationService(s.GetRequiredService<RedisDbService<Organisation>>()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapOrgEndpoints();

app.MapPost("organisation-gen-fake-data", async (int count, OrganisationDataFakeGenerator generator) =>
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
