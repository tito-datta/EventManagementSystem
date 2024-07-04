using data_access.cosmos;
using Microsoft.Azure.Cosmos;
using Microsoft.OpenApi.Models;
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
builder.Services.AddSingleton(new CosmosDbService<user_service.User>(userdb.GetSection("ConnectionString").Value!,
                                                                     userdb.GetSection("DatabaseName").Value!,
                                                                     userdb.GetSection("ContainerName").Value!));
builder.Services.AddScoped(s => new UserService(s.GetRequiredService<CosmosDbService<user_service.User>>()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "User Services v1"));
}

app.UseHttpsRedirection();

#region User APIs
app.MapGet("/users", async (UserService svc) =>
{
    var result = await svc.GetAsync();

    if (result.Error is not null && result.Content.GetType() == typeof(CosmosException))
    {
        return Results.Problem(result.Error);
    }

    return Results.Ok(result.Content as user_service.User[]);
}).WithName("GetAllUsers").WithOpenApi();

app.MapGet("/user/{id}/{organisationId}", async (string id, string organisationId, UserService svc) =>
{
    var result = await svc.GetByIdAsync(id, organisationId);

    if(result.Error is not null && result.Content.GetType() == typeof(CosmosException))
    {
        return Results.Problem(result.Error);
    }

    return Results.Ok(result.Content as user_service.User);
}).WithName("GetUserById").WithOpenApi();

app.MapPost("/user", async (user_service.User user, UserService svc) =>
{
    var result = await svc.CreateAsync(user);

    if (result.Error is not null && result.Content.GetType() == typeof(CosmosException))
    {
        return Results.Problem(result.Error);
    }

    return Results.Created();
}).WithName("CreateUser").WithOpenApi();

app.MapPut("/user", async (user_service.User user, UserService svc) =>
{
    var result = await svc.UpdateAsync(user);

    if (result.Error is not null && result.Content.GetType() == typeof(CosmosException))
    {
        return Results.Problem(result.Error);
    }

    return Results.Accepted();
});

app.MapDelete("/user", async (string id, string orgId, UserService svc) =>
{
    var result = await svc.DeleteUserAsync(id, orgId);

    if (result.Error is not null && result.Content.GetType() == typeof(CosmosException))
    {
        return Results.Problem(result.Error);
    }

    return Results.NotFound();
});
#endregion

app.Run();