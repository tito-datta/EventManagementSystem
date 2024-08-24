﻿using Microsoft.AspNetCore.Mvc;
using models;
using org_service;

namespace org_api
{
    public static class OrganisationEndpoints
    {
        public static void MapOrgEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/organisations", GetOrganisations)
               .WithName(nameof(GetOrganisations))
               .WithOpenApi();
            app.MapGet("/organisation/{name}", GetOrganisdationByName)
               .WithName(nameof(GetOrganisdationByName))
               .WithOpenApi();
            app.MapGet("/organisation/ID/{id}", GetOrganisdationById)
               .WithName(nameof(GetOrganisdationById))
               .WithOpenApi();
            app.MapPost("/organisation", AddOrganisation)
               .WithName(nameof(AddOrganisation))
               .WithOpenApi();
            app.MapPut("/organisation", ModifyOrganisation)
               .WithName(nameof(ModifyOrganisation))
               .WithOpenApi();
            // need something for PATCH operations
            app.MapDelete("/organisation/{name}", DeleteOrganisationByName)
               .WithName(nameof(DeleteOrganisationByName))
               .WithOpenApi();
            app.MapDelete("/organisation/{name}/{organisationId}", DeleteOrganisationById)
               .WithName(nameof(DeleteOrganisationById))
               .WithOpenApi();
        }

        private static async Task<IResult> GetOrganisdationById(string organisationId, OrganisationService svc)
        {
            if (organisationId is null || organisationId == string.Empty) return Results.BadRequest();

            var organisationsResult = (await svc.GetByIdAsync(organisationId));

            if (organisationsResult.Error is not null && organisationsResult.Content.GetType() == typeof(Exception))
            {
                return Results.Problem(organisationsResult.Error);
            }            

            if(organisationsResult == null || organisationsResult.Content is not Organisation)
            {                
                return Results.NotFound();
            }

            return Results.Ok(organisationsResult);
        }

        private static async Task<IResult> DeleteOrganisationByName(string name, OrganisationService svc)
        {
            if (name is null || name == string.Empty) return Results.BadRequest();

            var result = await svc.DeleteOrganisationAsync(name);

            if (result.Error is not null && result.Content.GetType() == typeof(Exception))
            {
                return Results.Problem(result.Error);
            }

            return Results.NotFound();
        }

        private static async Task<IResult> DeleteOrganisationById(string name, string orgId, OrganisationService svc)
        {
            if (name is null || name == string.Empty) return Results.BadRequest();
            if (orgId is null || orgId == string.Empty) return Results.BadRequest();

            var result = await svc.DeleteOrganisationAsync(name, orgId);

            if (result.Error is not null && result.Content.GetType() == typeof(Exception))
            {
                return Results.Problem(result.Error);
            }

            return Results.NotFound();
        }

        private static async Task<IResult> ModifyOrganisation(Organisation org, OrganisationService svc)
        {
            if (org is null) return Results.BadRequest();

            var result = await svc.UpdateOrganisationAsync(org);

            if (result.Error is not null && result.Content.GetType() == typeof(Exception))
            {
                return Results.Problem(result.Error);
            }

            return Results.Accepted();
        }

        private static async Task<IResult> AddOrganisation(Organisation org, OrganisationService svc)
        {
            if (org is null) return Results.BadRequest();

            var result = await svc.CreateOrganisationAsync(org);

            if (result.Error is not null && result.Content.GetType() == typeof(Exception))
            {
                return Results.Problem(result.Error);
            }

            return Results.Accepted();
        }

        private static async Task<IResult> GetOrganisdationByName(string name, OrganisationService svc)
        {
            if (name is null || name == string.Empty) return Results.BadRequest();

            var result = await svc.GetByNameAsync(name);

            if (result.Error is not null && result.Content.GetType() == typeof(Exception))
            {
                return Results.Problem(result.Error);
            }

            return Results.Ok(result.Content);
        }

        private static async Task<IResult> GetOrganisations(OrganisationService svc)
        {
            var result = await svc.GetAsync();

            if (result.Error is not null && result.Content.GetType() == typeof(Exception))
            {
                return Results.Problem(result.Error);
            }

            if (result.Content is null)
            {
                return Results.NotFound();
            }

            return Results.Ok(result.Content);
        }
    }
}
