using data_access;
using data_access.cosmos;
using data_access.redis;
using System.Text.Json.Serialization;

namespace user_service
{
    // TO-DO: Exception handling lol, this gonna break soon
    public class UserService
    {
        //private readonly CosmosDbService<User> _dbSvc;        
        //public UserService(CosmosDbService<User> dbSvc)
        //{
        //    _dbSvc = dbSvc;
        //}

        private readonly RedisCacheService<User> _dbSvc;
        public UserService(RedisCacheService<User> dbSvc)
        {
            _dbSvc = dbSvc;
        }

        public async Task<Result> GetAsync()
        {
            try
            {
                var result = await _dbSvc.GetAllAsync();
                return new() { Content = result };
            }
            catch (Exception ex)
            {
                return new() { Error = ex.Message, Content = ex };
            }
        }

        public async Task<Result> GetByIdAsync(string id, string organisationId) 
        {
            if(organisationId is null) throw new ArgumentNullException(nameof(organisationId));
            if(id is null) throw new ArgumentNullException(nameof(id));

            try
            {
                var result = await _dbSvc.GetAsync(id, organisationId);
                return new() { Content = result };
            }
            catch (Exception ex)
            {
                return new() { Error = ex.Message, Content = ex };
            }
        }

        public async Task<Result> CreateAsync(User user)
        {
            if (user is null) throw new ArgumentNullException(nameof(user));

            try
            {
                await _dbSvc.CreateAsync(user);
                return new() { Content = "Successfully saved." };
            }
            catch (Exception ex)
            {
                // gotta figure out what to do here
                return new() { Error = ex.Message, Content = ex };
            }
        }

        public async Task<Result> DeleteUserAsync(string id, string orgId)
        {
            if (id is null) throw new ArgumentNullException(nameof(id));

            try
            {
                var toDelete = await _dbSvc.GetAsync(id, orgId);

                if (toDelete is not null)
                {
                    await _dbSvc.DeleteAsync(toDelete);
                    return new() { Content = "Successfully deleted." };
                }
                return new() { Error = $"Object with id {id} was not found under organisation {orgId}" };
            }
            catch (Exception ex)
            {
                return new() { Error = ex.Message, Content = ex };
            }
        }

        public async Task<Result> UpdateAsync(User item)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));

            try
            {
                await _dbSvc.UpdateAsync(item);
                return new() { Content = $"Successfully updated item with id: {item.Id} under organisation: {item.OrganisationId}." };
            }
            catch (Exception ex)
            {
                return new() { Error = ex.Message, Content = ex };
            }

        }
    }
}    

