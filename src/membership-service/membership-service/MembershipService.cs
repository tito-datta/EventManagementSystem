using data_access;
using data_access.redis.database;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MebershipService
{
    public class MembershipService 
    {
        private readonly RedisDbService<Member> _dbSvc;
        public MembershipService(RedisDbService<Member> dbSvc)
        {
            _dbSvc = dbSvc;
        }

        public async Task<Result> GetAllMembersForAnOrgAsync(string organisationId)
        {
            if(organisationId is null) throw new ArgumentNullException(nameof(organisationId));

            try
            {
                //read from cache
                var members = await _dbSvc.GetAllAsync();
                if (members is not null && members.Count() > 0)
                {
                    // add to cache
                    var orgMembers = members.Where(m => m.OrganisationId == organisationId);
                    if (orgMembers is null && orgMembers?.Count() is 0)
                    {
                        return new() { Error = $"No memebers found for {organisationId}." };
                    }
                    return new() { Content = orgMembers! };
                }
                return new() { Error = $"No memebers found in db." };
            }
            catch (Exception ex)
            {
                return new Result() { Error = ex.Message, Content = ex };
            }
        }

        public async Task<Result> GetMembersByIdAndOrganisationAsync(string id, string organisationId)
        {
            if (id is null) throw new ArgumentNullException(nameof(id));
            if (organisationId is null) throw new ArgumentNullException(nameof(organisationId));

            try
            {
                // read from cache
                var member = await _dbSvc.GetAsync(id, organisationId);
                if (member is null)
                {
                    return new() { Error = $"No member was found with id: {id} under organisation {organisationId}." };
                }
                return new() { Content = member };
            }
            catch (Exception ex)
            {
                return new() { Content = ex, Error = ex.Message };
            }
        }

        public async Task<Result> CreateNewMemberAsync(Member member)
        {
            if (member is null) throw new ArgumentNullException(nameof(member));
            if (member.Id is null) throw new ArgumentNullException(nameof(member.Id));
            if (member.OrganisationId is null) throw new ArgumentNullException(nameof(member.OrganisationId));
            if (member.Name is null) throw new ArgumentNullException(nameof(member.Name));
            if (member.PhoneNumber is null) throw new ArgumentNullException(nameof(member.PhoneNumber));

            try
            {
                await _dbSvc.CreateAsync(member);
                return new() { Content = "Successfully created member." };
            }
            catch (Exception ex) 
            {
                return new() { Content = ex, Error = ex.Message};
            }
        }

        public async Task<Result> UpdateMemberAsync(Member member)
        {
            if (member is null) throw new ArgumentNullException(nameof(member));
            if (member.Id is null) throw new ArgumentNullException(nameof(member.Id));
            if (member.OrganisationId is null) throw new ArgumentNullException(nameof(member.OrganisationId));
            if (member.Name is null) throw new ArgumentNullException(nameof(member.Name));
            if (member.PhoneNumber is null) throw new ArgumentNullException(nameof(member.PhoneNumber));

            try
            {
                await _dbSvc.UpdateAsync(member);
                return new() { Content = "Successfully updated member." };
            }
            catch (Exception ex)
            {
                return new() { Content = ex, Error = ex.Message };
            }
        }

        public async Task<Result> DeleteMemberAsync(string id, string organisationId)
        {            
            if (id is null) throw new ArgumentNullException(nameof(id));
            if (organisationId is null) throw new ArgumentNullException(nameof(organisationId));

            try
            {
                var toDelete = await GetMembersByIdAndOrganisationAsync(id, organisationId);

                if (toDelete is not null)
                {
                    await _dbSvc.DeleteAsync((Member)toDelete.Content);
                    return new() { Content = "Successfully deleted Member." };
                }
                return new() { Error = $"Member with id {id} was not found under organisation {organisationId}" };
            }
            catch (Exception ex)
            {
                return new() { Error = ex.Message, Content = ex };
            }
        }
    }
}
