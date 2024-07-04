using Microsoft.Azure.Cosmos;
using user_service;

namespace user_api
{
    public static class UserEndpoints
    {
        public static void MapUserEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/users", GetUsers)
               .WithName(nameof(GetUsers))
               .WithOpenApi();
            app.MapGet("/user/{id}/{organisationId}", GetUserById)
               .WithName(nameof(GetUserById))
               .WithOpenApi();
            app.MapPost("/user", AddUser)
               .WithName(nameof(AddUser))
               .WithOpenApi();
            app.MapPut("/user", ModifyUser)
               .WithName(nameof(ModifyUser))
               .WithOpenApi();
            app.MapDelete("/user", DeleteUser)
               .WithName(nameof(DeleteUser))
               .WithOpenApi();
        }

        public static async Task<IResult> DeleteUser(string id, string orgId, UserService svc)
        {
            var result = await svc.DeleteUserAsync(id, orgId);

            if (result.Error is not null && result.Content.GetType() == typeof(CosmosException))
            {
                return Results.Problem(result.Error);
            }

            return Results.NotFound();
        }

        public static async Task<IResult> ModifyUser(user_service.User user, UserService svc)
        {
            var result = await svc.UpdateAsync(user);

            if (result.Error is not null && result.Content.GetType() == typeof(CosmosException))
            {
                return Results.Problem(result.Error);
            }

            return Results.Accepted();
        }

        public static async Task<IResult> AddUser(user_service.User user, UserService svc)
        {
            var result = await svc.CreateAsync(user);

            if (result.Error is not null && result.Content.GetType() == typeof(CosmosException))
            {
                return Results.Problem(result.Error);
            }

            return Results.Created();
        }

        public static async Task<IResult> GetUserById(string id, string organisationId, UserService svc)
        {
            var result = await svc.GetByIdAsync(id, organisationId);

            if (result.Error is not null && result.Content.GetType() == typeof(CosmosException))
            {
                return Results.Problem(result.Error);
            }

            return Results.Ok(result.Content as user_service.User);
        }

        public static async Task<IResult> GetUsers(UserService svc)
        {
            var result = await svc.GetAsync();

            if (result.Error is not null && result.Content.GetType() == typeof(CosmosException))
            {
                return Results.Problem(result.Error);
            }

            return Results.Ok(result.Content as user_service.User[]);
        }
    }
}
