using data_access.redis.database;
using Microsoft.AspNetCore.Connections;
using Microsoft.OpenApi.Models;
using models;
using org_api;
using org_service;
using Redis.OM;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddEndpointsApiExplorer();

if (!builder.Environment.IsDevelopment())
{
    builder.Configuration.Sources.Clear();
    builder.Configuration.AddEnvironmentVariables();
}

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Organisation Services", Version = "v1" });
});

// Register Redis
builder.Services.AddSingleton(new RedisDbService<Organisation>(new RedisConnectionProvider(builder.Configuration.GetSection("Redis").Value!)));

// Register services
builder.Services.AddScoped(s => new OrganisationService(s.GetRequiredService<RedisDbService<Organisation>>()));

var app = builder.Build();

//create index
//to-do: check if index exists
var connectionProvider = new RedisConnectionProvider(app.Configuration["Redis"]!);
connectionProvider.Connection.CreateIndex(typeof(Organisation));

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options = new()
        {
            DocumentTitle = "Organisation services"
        };
    });
    app.UseDeveloperExceptionPage();
}

// Middlewares
// Example middleware
app.Use(async (ctx, next) =>
{
    Stopwatch watch = new();
    watch.Start();
    ctx.Response.OnCompleted(() =>
    {
        watch.Stop();
        return Task.CompletedTask;
    });
    _ = ctx.Response.Headers.TryAdd("X-Response-Time", string.Format("{0} ms", watch.ElapsedMilliseconds.ToString()));
    await next.Invoke();
});

app.UseHttpsRedirection();

app.MapOrgEndpoints();

app.Run();
