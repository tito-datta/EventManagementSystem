namespace data_access.cosmos
{
    public interface ICosmosEntity
    {
        string CosmosEntityName { get; }
        // partiotion decesion is creeping into code but too lazy to refactor
        string PartitionKey { get; }
        string id { get; }
    }
}