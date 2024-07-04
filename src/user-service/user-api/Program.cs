using data_access.cosmos;
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

app.MapUserEndpoints();

app.Run();