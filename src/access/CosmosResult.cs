using access;

public record CosmosResult<T> where T : ICosmosEntity
{
    public CosmosResult(string[] Errors)
    {
        this.Errors = Errors;
    }

    public CosmosResult(T Item)
    {
        this.Item = Item;
    }

    public CosmosResult(T item, string[] errors) : this(item)
    {
        Errors = errors;
    }

    public T Item { get; set; }
    public string[] Errors { get; set; }
}