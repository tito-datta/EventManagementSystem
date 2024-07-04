using System.Text.Json.Serialization;

namespace data_access.cosmos
{
    public record CosmosEntity : ICosmosEntity
    {
        public string CosmosEntityName { get; }

        public string PartitionKey { get; }

        [JsonPropertyName("id")]
        public string id { get; }
    }
}
