public class Item
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required float Price { get; set; }
    public required int Stock { get; set; }
    public required string Category { get; set; }
    public required string StoreId { get; set; }

    // Methods
    public void UpdateStock(int quantity)
    {
        Stock = Stock + quantity;
    }
    public void ChangePrice(float newPrice)
    {
        Price = newPrice;
    }
}