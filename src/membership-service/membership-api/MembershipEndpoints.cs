using MebershipService;
using Microsoft.Azure.Cosmos;
using System.Security.Cryptography;

namespace membership_api
{
    public static class MembershipEndpoints
    {
        public static void MapMembershipEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/members/{organisationId}", GetAllMembersForAnOrganisation)
                   .WithName(nameof(GetAllMembersForAnOrganisation))
                   .WithOpenApi();
            app.MapGet("/members/{id}/{organisationId}", GetMembersByIdAndOrganisation)
                   .WithName(nameof(GetMembersByIdAndOrganisation))
                   .WithOpenApi();
            app.MapPut("/members", ModyfyMember)
                   .WithName(nameof(ModyfyMember))
                   .WithOpenApi();
            app.MapDelete("/members", DeleteMember)
                   .WithName(nameof(DeleteMember))
                   .WithOpenApi();
            app.MapPost("/members", CreateMember)
                   .WithName(nameof(CreateMember))
                   .WithOpenApi();
        }

        public static async Task<IResult> GetAllMembersForAnOrganisation(string organisationId, MembershipService svc)
        {
            var result = await svc.GetAllMembersForAnOrgAsync(organisationId);

            if (result.Error is not null || result.Content?.GetType() == typeof(CosmosException))
            {
                return Results.Problem(result.Error);
            }

            return Results.Ok(result.Content);
        }

        public static async Task<IResult> GetMembersByIdAndOrganisation(string id, string organisationId, MembershipService svc)
        {
            var result = await svc.GetMembersByIdAndOrganisationAsync(id, organisationId);

            if (result.Error is not null || result.Content?.GetType() == typeof(CosmosException))
            {
                return Results.Problem(result.Error);
            }

            return Results.Ok(result.Content);
        }

        public static async Task<IResult> DeleteMember(string id, string organisationId, MembershipService svc)
        {
            var result = await svc.DeleteMemberAsync(id, organisationId);

            if (result.Error is not null || result.Content.GetType() == typeof(CosmosException))
            {
                return Results.Problem(result.Error);
            }

            return Results.NotFound();
        }

        public static async Task<IResult> ModyfyMember(Member member, MembershipService svc)
        {
            var result = await svc.UpdateMemberAsync(member);

            if (result.Error is not null || result.Content.GetType() == typeof(CosmosException))
            {
                return Results.Problem(result.Error);
            }

            return Results.Accepted();
        }

        public static async Task<IResult> CreateMember(Member member, MembershipService svc)
        {
            var result = await svc.CreateNewMemberAsync(member);

            if (result.Error is not null || result.Content.GetType() == typeof(CosmosException))
            {
                return Results.Problem(result.Error);
            }

            return Results.Created();
        }
    }
}
