
using Bogus.DataSets;
using Microsoft.Azure.Cosmos;
using org_service;
using System.Security.Cryptography;

namespace org_api
{
    public static class OrganisationEndpoints
    {
        public static void MapOrgEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/organisations", GetOrganisations)
               .WithName(nameof(GetOrganisations))
               .WithOpenApi();
            app.MapGet("/organisation/{name}", GetOrgByName)
               .WithName(nameof(GetOrgByName))
               .WithOpenApi();
            app.MapPost("/organisation", AddOrganisation)
               .WithName(nameof(AddOrganisation))
               .WithOpenApi();
            app.MapPut("/organisation", ModifyOrganisation)
               .WithName(nameof(ModifyOrganisation))
               .WithOpenApi();
            app.MapDelete("/organisation/{name}", DeleteOrganisationByName)
               .WithName(nameof(DeleteOrganisationByName))
               .WithOpenApi();
            app.MapDelete("/organisation/{name}/{organisationId}", DeleteOrganisationById)
               .WithName(nameof(DeleteOrganisationById))
               .WithOpenApi();
        }

        private static async Task<IResult> DeleteOrganisationByName(string name, OrganisationService svc)
        {
            var result = await svc.DeleteOrganisationAsync(name);

            if (result.Error is not null && result.Content.GetType() == typeof(CosmosException))
            {
                return Results.Problem(result.Error);
            }

            return Results.NotFound();
        }

        private static async Task<IResult> DeleteOrganisationById(string name, string orgId, OrganisationService svc)
        {
            var result = await svc.DeleteOrganisationAsync(name, orgId);

            if (result.Error is not null && result.Content.GetType() == typeof(CosmosException))
            {
                return Results.Problem(result.Error);
            }

            return Results.NotFound();
        }

        private static async Task<IResult> ModifyOrganisation(Organisation org, OrganisationService svc)
        {
            var result = await svc.UpdateOrganisationAsync(org);

            if (result.Error is not null && result.Content.GetType() == typeof(CosmosException))
            {
                return Results.Problem(result.Error);
            }

            return Results.Accepted();
        }

        private static async Task<IResult> AddOrganisation(Organisation org, OrganisationService svc)
        {
            var result = await svc.CreateOrganisationAsync(org);

            if (result.Error is not null && result.Content.GetType() == typeof(CosmosException))
            {
                return Results.Problem(result.Error);
            }

            return Results.Accepted();
        }

        private static async Task<IResult> GetOrgByName(string name, OrganisationService svc)
        {
            var result = await svc.GetByNameAsync(name);

            if (result.Error is not null && result.Content.GetType() == typeof(CosmosException))
            {
                return Results.Problem(result.Error);
            }

            return Results.Ok(result.Content);
        }

        private static async Task<IResult> GetOrganisations(OrganisationService svc)
        {
            var result = await svc.GetAsync();

            if (result.Error is not null && result.Content.GetType() == typeof(CosmosException))
            {
                return Results.Problem(result.Error);
            }

            return Results.Ok(result.Content);
        }
    }
}
