using data_access.cosmos;

namespace user_service
{
    public record User : ICosmosEntity
    {
        //private string ID;

        public string id { get; set; } = Guid.NewGuid().ToString();
        //public string id 
        //{
        //    get
        //    {
        //        return ID;
        //    }
        //    set
        //    {
        //        if (value is null)
        //        {
        //            ID = Guid.NewGuid().ToString();
        //        }
        //        else
        //        {
        //            ID = value;
        //        }
        //    }
        //}

        public string OrganisationId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string CosmosEntityName { get; } = "User";

        public string PartitionKey => OrganisationId;
    }
}
