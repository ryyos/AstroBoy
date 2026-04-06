public class Item
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required float Price { get; set; }
    public required int Stock { get; set; }
    public required string Category { get; set; }
    public required string StoreId { get; set; }

    public void UpdateStock()
    {
    }

    public void ChangePrice()
    {
    }
}