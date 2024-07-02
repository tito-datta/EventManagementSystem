using access;

namespace user_access
{
    public sealed class UserDbAccess : CosmosDb<UserEntity>
    {        
        private static readonly object _lock = new object();
        private static UserDbAccess _instance;
        private UserDbAccess(string connectionString,
                            string databaseName,
                            string containerName) : base(connectionString, databaseName, containerName)
        {
        }

        public static UserDbAccess Instance(string connectionString, string databaseName, string containerName)
        {
            if (_instance is null)
            {
                lock (_lock)
                {
                    if (_instance is null) return new UserDbAccess(connectionString, databaseName, containerName);
                }
            }
            return _instance;
        }

        public async Task<UserEntity> GetUserAsync(string userid, string partitionkey)
        {
            var response = await ReadItemAsync(userid, partitionkey);

            if (response.Errors.Any())
            {
                // retry logic goes here
                throw new NotImplementedException("Unhappy path is missing.");
            }

            return response.Item;
        }

        public async Task<bool> AddUser(UserEntity user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            var response = await CreateItemAsync(user);
            if (response.Errors.Any())
            {
                // retry logic goes here
                throw new NotImplementedException("Unhappy path is missing.");
            }

            return true;
        }

        public async Task<UserEntity> UpdateUser(UserEntity user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            var response = await UpdateItemAsync(user);
            if (response.Errors.Any())
            {
                // retry logic goes here
                throw new NotImplementedException("Unhappy path is missing.");
            }

            return response.Item;
        }

        public async Task DeleteUser(UserEntity user)
        {
            if(user == null) throw new ArgumentNullException( nameof(user));

            await DeleteItemAsync(user.Id, user.PartitionKey);
        }
    }
}
