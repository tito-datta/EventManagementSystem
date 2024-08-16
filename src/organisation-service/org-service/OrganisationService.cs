using data_access;
using data_access.redis.cache;
using data_access.redis.database;
using models;
using System.Collections.Immutable;

namespace org_service
{
    public class OrganisationService
    {
        private readonly RedisDbService<Organisation> _dbSvc;

        public OrganisationService(RedisDbService<Organisation> dbSvc)
        {
            _dbSvc = dbSvc;
        }

        public async Task<Result> GetAsync()
        {
            try
            {
                var result = await _dbSvc.QueryAsync(a => Task.FromResult(a.ToArray()));
                return new() { Content = result };
            }
            catch (Exception ex)
            {
                return ex.Message.Contains("no such index") 
                       ? new() { Content = null } 
                       : new() { Error = ex.Message, Content = ex };
            }
        }

        public async Task<Result> GetByNameAsync(string name)
        {
            try
            {
                var all = await _dbSvc.QueryAsync(o => Task.FromResult(o.Where(oo => oo.Name == name).ToArray()));
                if(all is not null && all.Length > 0)
                {                    
                    return new() { Content = all };
                }
                return new() { Content = string.Format("No match found for {0}.", name) };
            }
            catch (Exception ex)
            {
                return new() { Error = ex.Message, Content = ex };
            }
        }

        public async Task<Result> CreateOrganisationAsync(Organisation organisation)
        {
            if (organisation == null) throw new ArgumentNullException(nameof(organisation));

            try
            {
                await _dbSvc.CreateAsync(organisation);
                return new() { Content = "Successfully saved." };
            }
            catch (Exception ex)
            {
                // gotta figure out what to do here
                return new() { Error = ex.Message, Content = ex };
            }
        }

        public async Task<Result> DeleteOrganisationAsync(string name, string organisationId = null)
        {
            if(name == null) throw new ArgumentNullException(nameof(name));
            
            try
            {
                var toDelete = await _dbSvc. QueryAsync(o => Task.FromResult(o.Where(oo => oo.Name == name).ToArray()));
                if(toDelete is not null && toDelete.Length is not 0)
                {
                    if (toDelete.Length > 1)
                    {
                        throw new ArgumentNullException(string.Format("More than one matches fournd for {0}, provide {1} parameter to delete.", name, organisationId));
                    }

                    await _dbSvc.DeleteAsync(toDelete.Single());
                    return new() { Content = "Successfully deleted." };
                }
                return new() { Content = string.Format("No match found for {0}.", name) };
            }
            catch (Exception ex)
            {
                return new() { Error = ex.Message, Content = ex };
            }
        }

        public async Task<Result> UpdateOrganisationAsync(Organisation organisation)
        {
            if (organisation == null) throw new ArgumentNullException(nameof(organisation));

            try
            {
                await _dbSvc.UpdateAsync(organisation);
                return new() { Content = $"Successfully updated {organisation.Name}." };
            }
            catch (Exception ex)
            {
                return new() { Error = ex.Message, Content = ex };
            }
        }        

        public async Task<Result> GetAllUsersForAnOrgAsync(string orgId)
        {
            if(string.IsNullOrEmpty(orgId)) throw new ArgumentNullException(nameof(orgId));

            try
            {
                var users = await _dbSvc.QueryAsync(org => Task.FromResult((from o in org
                                                                            where o.Id.ToString() == orgId
                                                                            select o.Users).ToArray()));

                if (users == null|| users.Length == 0)
                {
                    return new() { Content = string.Format("Organisation with id {0} has no users.", orgId) };
                }

                return new() { Content = users };
            }
            catch (Exception ex)
            {
                return new() { Error = ex.Message, Content = ex };
            }
        }

        public async Task<Result> GetAllMembersForAnOrgAsync(string orgId)
        {
            if (string.IsNullOrEmpty(orgId)) throw new ArgumentNullException(nameof(orgId));

            try
            {
                var members = await _dbSvc.QueryAsync(org => Task.FromResult((from o in org
                                                                                   where o.Id.ToString() == orgId
                                                                                   select o.Members).ToArray()));

                if (members == null || members.Length == 0)
                {
                    return new() { Content = string.Format("Organisation with id {0} has no members.", orgId) };
                }

                return new() { Content = members };
            }
            catch (Exception ex)
            {
                return new() { Error = ex.Message, Content = ex };
            }
        }

        public async Task<Result> GetUserDetailsAsync(string userName, string orgId)
        {
            if (string.IsNullOrEmpty(orgId)) throw new ArgumentNullException(nameof(orgId));

            try
            {
                var users = await _dbSvc.QueryAsync(org => Task.FromResult((from o in org 
                                                                            where o.Id.ToString() == orgId 
                                                                                from usr in o.Users 
                                                                                where usr.Name == userName 
                                                                            select usr).ToArray()));

                if (users == null || users.Length == 0)
                {
                    return new() { Content = string.Format("Organisation with id {0} has no users.", orgId) };
                }

                return new() { Content = users.Length > 1 ? users : users.Single() };
            }
            catch (Exception ex)
            {
                return new() { Error = ex.Message, Content = ex };
            }
        }

        public async Task<Result> GeMemberDetailsAsync(string memberName, string orgId)
        {
            if (string.IsNullOrEmpty(orgId)) throw new ArgumentNullException(nameof(orgId));

            try
            {
                var members = await _dbSvc.QueryAsync(o => Task.FromResult(query(memberName, orgId, o)));

                if (members == null || members.Length == 0)
                {
                    return new() { Content = string.Format("Organisation with id {0} has no members.", orgId) };
                }

                return new() { Content = members.Length > 1 ? members : members.Single() };
            }
            catch (Exception ex)
            {
                return new() { Error = ex.Message, Content = ex };
            }

            Member[] query(string memberName,
                           string orgId,
                           System.Collections.Immutable.ImmutableArray<Organisation> o)
            {
                var result = from org in o 
                             where org.Id.ToString() == orgId 
                                from member in org.Members 
                                where member.Name == memberName 
                             select member;

                return result.ToArray();
            }
        }
    }
}
