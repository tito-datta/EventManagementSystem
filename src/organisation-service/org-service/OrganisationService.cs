using DataAccess;
using DataAccess.Redis.Database;
using models;

namespace OrganisationService;

public class OrganisationService
{
    private readonly RedisDbService<Organisation> _dbSvc;

    public OrganisationService(RedisDbService<Organisation> dbSvc) =>
        _dbSvc = dbSvc ?? throw new ArgumentNullException(nameof(dbSvc));

    /// <summary>
    /// Retrieves all organisations.
    /// </summary>
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

    /// <summary>
    /// Retrieves organisations by name.
    /// </summary>
    public async Task<Result> GetByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name));

        try
        {
            var all = await _dbSvc.QueryAsync(o => Task.FromResult(o.Where(oo => oo.Name == name).ToArray()));
            return all is not null && all.Length > 0
                ? new Result { Content = all }
                : new Result { Content = $"No match found for {name}." };
        }
        catch (Exception ex)
        {
            return CreateErrorResult(ex);
        }
    }

    /// <summary>
    /// Retrieves an organisation by ID.
    /// </summary>
    public async Task<Result> GetByIdAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentNullException(nameof(id));

        try
        {
            var organisation = await _dbSvc.QueryAsync(o => Task.FromResult(new Organisation[] { o.SingleOrDefault(oo => oo.Id.ToString() == id) }));

            return organisation != null
                ? new Result { Content = organisation }
                : new Result { Content = default };
        }
        catch (Exception ex)
        {
            return CreateErrorResult(ex);
        }
    }

    /// <summary>
    /// Creates a new organisation.
    /// </summary>
    public async Task<Result> CreateOrganisationAsync(Organisation organisation)
    {
        if (organisation == null)
            throw new ArgumentNullException(nameof(organisation));

        try
        {
            await _dbSvc.CreateAsync(organisation);
            return new Result { Content = "Successfully saved." };
        }
        catch (Exception ex)
        {
            return CreateErrorResult(ex);
        }
    }

    /// <summary>
    /// Deletes an organisation by name and optional ID.
    /// </summary>
    public async Task<Result> DeleteOrganisationAsync(string name, string organisationId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name));

        try
        {
            var toDelete = await _dbSvc.QueryAsync(o => Task.FromResult(o.Where(oo => oo.Name == name).ToArray()));

            if (toDelete.Length == 0)
                return new Result { Content = $"No match found for {name}." };

            if (toDelete.Length > 1 && string.IsNullOrWhiteSpace(organisationId))
                throw new ArgumentException($"More than one match found for {name}, provide organisationId parameter to delete.");

            var orgToDelete = organisationId != null
                ? toDelete.Single(o => o.Id.ToString() == organisationId)
                : toDelete.Single();

            await _dbSvc.DeleteAsync(orgToDelete);
            return new Result { Content = "Successfully deleted." };
        }
        catch (Exception ex)
        {
            return CreateErrorResult(ex);
        }
    }

    /// <summary>
    /// Updates an existing organisation.
    /// </summary>
    public async Task<Result> UpdateOrganisationAsync(Organisation organisation)
    {
        if (organisation == null)
            throw new ArgumentNullException(nameof(organisation));

        try
        {
            await _dbSvc.UpdateAsync(organisation);
            return new Result { Content = $"Successfully updated {organisation.Name}." };
        }
        catch (Exception ex)
        {
            return CreateErrorResult(ex);
        }
    }

    /// <summary>
    /// Retrieves all users for an organisation.
    /// </summary>
    public async Task<Result> GetAllUsersForAnOrgAsync(string orgId)
    {
        if (string.IsNullOrEmpty(orgId))
            throw new ArgumentNullException(nameof(orgId));

        try
        {
            var users = await _dbSvc.QueryAsync(org =>
                Task.FromResult(org.FirstOrDefault(o => o.Id.ToString() == orgId)?.Users ?? Array.Empty<User>()));

            return users.Length == 0
                ? new Result { Content = $"Organisation with id {orgId} has no users." }
                : new Result { Content = users };
        }
        catch (Exception ex)
        {
            return CreateErrorResult(ex);
        }
    }

    /// <summary>
    /// Retrieves all members for an organisation.
    /// </summary>
    public async Task<Result> GetAllMembersForAnOrgAsync(string orgId)
    {
        if (string.IsNullOrEmpty(orgId))
            throw new ArgumentNullException(nameof(orgId));

        try
        {
            var members = await _dbSvc.QueryAsync(org =>
                Task.FromResult(org.FirstOrDefault(o => o.Id.ToString() == orgId)?.Members ?? Array.Empty<Member>()));

            return members.Length == 0
                ? new Result { Content = $"Organisation with id {orgId} has no members." }
                : new Result { Content = members };
        }
        catch (Exception ex)
        {
            return CreateErrorResult(ex);
        }
    }

    /// <summary>
    /// Retrieves user details for an organisation.
    /// </summary>
    public async Task<Result> GetUserDetailsAsync(string userName, string orgId)
    {
        if (string.IsNullOrEmpty(orgId))
            throw new ArgumentNullException(nameof(orgId));

        try
        {
            var users = await _dbSvc.QueryAsync(org => Task.FromResult(
                (from o in org
                 where o.Id.ToString() == orgId
                 from user in o.Users
                 where user.Name == userName
                 select user).ToArray()));

            return users.Length == 0
                ? new Result { Content = $"No user found with name {userName} in organisation with id {orgId}." }
                : new Result { Content = users.Length > 1 ? users : users[0] };
        }
        catch (Exception ex)
        {
            return CreateErrorResult(ex);
        }
    }

    /// <summary>
    /// Retrieves member details for an organisation.
    /// </summary>
    public async Task<Result> GetMemberDetailsAsync(string memberName, string orgId)
    {
        if (string.IsNullOrEmpty(orgId))
            throw new ArgumentNullException(nameof(orgId));

        try
        {
            var members = await _dbSvc.QueryAsync(org => Task.FromResult(
                (from o in org
                 where o.Id.ToString() == orgId
                 from member in o.Members
                 where member.Name == memberName
                 select member).ToArray()));

            return members.Length == 0
                ? new Result { Content = $"No member found with name {memberName} in organisation with id {orgId}." }
                : new Result { Content = members.Length > 1 ? members : members[0] };
        }
        catch (Exception ex)
        {
            return CreateErrorResult(ex);
        }
    }

    private Result CreateErrorResult(Exception ex) =>
        new Result { Error = ex.Message, Content = ex };

}