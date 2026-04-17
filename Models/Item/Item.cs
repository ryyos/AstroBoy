namespace AstroBoy.Models;

public class Item
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public float Price { get; set; }
    public int Stock { get; set; }
    public string Category { get; set; }
    public string StoreId { get; set; }
}