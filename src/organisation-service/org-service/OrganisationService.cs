using data_access;
using data_access.redis.database;

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
                var result = await _dbSvc.GetAllAsync();
                return new() { Content = result };
            }
            catch (Exception ex)
            {
                return new() { Error = ex.Message, Content = ex };
            }
        }

        public async Task<Result> GetByNameAsync(string name)
        {
            try
            {
                var all = await _dbSvc.GetAllAsync();
                if(all is not null && all.Length > 0)
                {
                    var result = all.Where(a => a.Name == name);
                    return new() { Content = result };
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
                var toDelete = await _dbSvc.GetAllAsync();
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
    }
}
